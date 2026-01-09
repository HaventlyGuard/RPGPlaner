using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DataAcces;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    // УДАЛИТЕ эти DbSet - Permission, PrivacySettings, UserSettings теперь ComplexType
    // public DbSet<Permission> Permissions { get; set; }
    // public DbSet<PrivacySettings> PrivacySettings { get; set; }
    // public DbSet<UserSettings> UserSettings { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Конфигурация Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);
            
            // Связь с родительской ролью
            entity.HasOne(r => r.ParentRole)
                  .WithMany(r => r.ChildRoles)
                  .HasForeignKey(r => r.ParentRoleId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
            
            // Пользователи с этой ролью
            entity.HasMany(r => r.Users)
                  .WithOne(u => u.Role)
                  .HasForeignKey(u => u.RoleId)
                  .IsRequired(false);
        });
        
        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(255);
            entity.Property(u => u.CreateAt).HasDefaultValueSql("NOW()");
            
            // Связь с RefreshToken (один-к-одному)
            entity.HasOne(u => u.RefreshTokens)
                  .WithOne(rt => rt.User)
                  .HasForeignKey<RefreshToken>(rt => rt.UserId)
                  .IsRequired(false);
        });
        
        // Конфигурация RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => rt.UserId).IsUnique();
            
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.CreatedByIp).IsRequired().HasMaxLength(45);
            entity.Property(rt => rt.ReplacedByToken).HasMaxLength(500);
            entity.Property(rt => rt.ReasonRevoked).HasMaxLength(200);
            
            // Автоматическое заполнение CreatedAt
            entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("NOW()");
        });
    }
}