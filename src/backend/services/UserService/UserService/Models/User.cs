using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("user_name")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("first_name")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Column("avatar", TypeName = "bytea")]
    public byte[]? Avatar { get; set; }

    [Column("avatar_url")]
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    [Column("avatar_size")]
    public long? AvatarSize { get; set; }

    [Column("avatar_mime_type")]
    [MaxLength(100)]
    public string? AvatarMimeType { get; set; }

    [Required]
    [EmailAddress]
    [Column("email")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("password_salt")]
    [MaxLength(255)]
    public string PasswordSalt { get; set; } = string.Empty;

    [Column("update_at")]
    public DateTime? UpdateAt { get; set; }

    [Column("create_at")]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    [Column("direct_permissions", TypeName = "jsonb")]
    public List<Permission> DirectPermissions { get; set; } = new();

    [Column("team_ids", TypeName = "uuid[]")]
    public List<Guid> TeamIds { get; set; } = new();

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_banned")]
    public bool IsBanned { get; set; } = false;

    [Column("banned_until")]
    public DateTime? BannedUntil { get; set; }

    [Column("ban_reason")]
    [MaxLength(500)]
    public string? BanReason { get; set; }

    [Column("role_id")]
    public Guid? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    [Column("permissions", TypeName = "text[]")]
    public List<string> Permissions { get; set; } = new();

    [Column("character_id")]
    public Guid CharacterId { get; set; }

    // ComplexType - будут сохранены как JSONB в таблице users
    [Column("settings", TypeName = "jsonb")]
    public UserSettings Settings { get; set; } = new();

    [Column("privacy_settings", TypeName = "jsonb")]
    public PrivacySettings PrivacySettings { get; set; } = new();

    // Навигационные свойства
    public virtual RefreshToken RefreshTokens { get; set; }
}