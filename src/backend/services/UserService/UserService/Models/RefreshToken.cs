using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Repository.Intefaces;

namespace UserService.Models;

[Table("refresh_tokens")]
public class RefreshToken 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [Required]
    [Column("token")]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("created_by_ip")]
    [MaxLength(45)]
    public Guid CreatedByIp { get; set; } = Guid.Empty;

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("reason_revoked")]
    [MaxLength(200)]
    public string? ReasonRevoked { get; set; }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [NotMapped]
    public bool IsActive => RevokedAt == null && !IsExpired;
}