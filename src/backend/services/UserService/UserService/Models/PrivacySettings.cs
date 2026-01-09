using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

// ComplexType - будет сохранен как часть User
[ComplexType]
public class PrivacySettings
{
    [Column("profile_public")]
    public bool ProfilePublic { get; set; } = true;

    [Column("show_email")]
    public bool ShowEmail { get; set; } = false;

    [Column("show_last_login")]
    public bool ShowLastLogin { get; set; } = true;

    [Column("show_achievements")]
    public bool ShowAchievements { get; set; } = true;

    [Column("show_level")]
    public bool ShowLevel { get; set; } = true;

    [Column("allow_friend_requests")]
    public bool AllowFriendRequests { get; set; } = true;

    [Column("allow_messages")]
    public bool AllowMessages { get; set; } = true;
}