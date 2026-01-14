using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DataAcces;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            // Явно указываем имя таблицы
            entity.ToTable("users");
            
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            
            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");
            
            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);
                  
            entity.Property(u => u.IsEmailVerified)
                .HasDefaultValue(false);
                  
            entity.Property(u => u.IsAdmin)
                .HasDefaultValue(false);
            
            // Для byte[] не нужно указывать TypeName в атрибутах, если указали в модели
            entity.Property(u => u.AvatarByte)
                .HasColumnType("bytea");
            
            entity.Property(u => u.AvatarType)
                .HasMaxLength(100);
            
            // Связь с RefreshTokens
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Конфигурация RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(rt => rt.Token).IsUnique();
            
            entity.Property(rt => rt.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}