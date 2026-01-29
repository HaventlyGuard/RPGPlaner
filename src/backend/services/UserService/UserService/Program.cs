using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using UserService.DataAcces;
using UserService.Services;
using UserService.Models;
using UserService.Repository;
using UserService.Repository.Intefaces;
using UserService.Services.Interfaces;
using UserService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
var jwtOptionsSection = configuration.GetSection(nameof(JwtOptions));
var secret = jwtOptionsSection["Secret"];
var issuer = jwtOptionsSection["Issuer"];
var audience = jwtOptionsSection["Audience"];

if (string.IsNullOrEmpty(secret))
    throw new InvalidOperationException("JWT Secret не настроен в appsettings.json");
if (string.IsNullOrEmpty(issuer))
    throw new InvalidOperationException("JWT Issuer не настроен в appsettings.json");
if (string.IsNullOrEmpty(audience))
    throw new InvalidOperationException("JWT Audience не настроен в appsettings.json");

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; 
        options.SaveToken = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Можно убрать задержку для точного времени
        };
    });

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")  
                .AllowAnyHeader()                     
                .AllowAnyMethod();                    
        });
});

// 🔴 УБЕРИТЕ ДУБЛИРОВАНИЕ - Swagger добавляется только один раз
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service API", Version = "v1" });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    

NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

var app = builder.Build();

// Middleware pipeline
app.UseCors("AllowFrontend");

// ✅ ДОБАВЬТЕ ЭТО - Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v1");
    c.RoutePrefix = "swagger"; // Устанавливает путь /swagger
});

// Миграции
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    dbContext.Database.Migrate(); 
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets(); // Если нужно
app.MapControllers();

app.Run();