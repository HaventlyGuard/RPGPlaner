using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("username")]
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

    [Column("avatar_url")]
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
    
    [Column("avatar_byte", TypeName = "bytea")]
    public byte[]? AvatarByte { get; set; }
    
    [Column("avatar_type", TypeName = "varchar")]
    public string? AvatarType { get; set; }
    

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

    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login")] 
    public DateTime? LastLogin { get; set; } = DateTime.UtcNow;

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_admin")]
    public bool IsAdmin { get; set; } = false; 
    [Column("settings", TypeName = "jsonb")]
    public UserSettings Settings { get; set; } = new();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}