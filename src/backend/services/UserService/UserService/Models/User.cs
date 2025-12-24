namespace UserService.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte[]? Avatar { get; set; } 
    public string? AvatarUrl { get; set; } = string.Empty;
    public long? AvatarSize { get; set; }
    public string? AvatarMimeType { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public DateTime? UpdateAt { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBanned { get; set; }
    public DateTime? BannedUntil { get; set; }
    public string? BanReason { get; set; }
    
    public UserRole Role { get; set; } = UserRole.User;
    public List<string> Permissions { get; set; } = new();
    
    public UserSettings Settings { get; set; } = new();
    
    public Guid CharacterId { get; set; } 
}