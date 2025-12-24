namespace UserService.Models;

public class PrivacySettings
{
    public bool ProfilePublic { get; set; } = true;
    public bool ShowEmail { get; set; } = false;
    public bool ShowLastLogin { get; set; } = true;
    public bool ShowAchievements { get; set; } = true;
    public bool ShowLevel { get; set; } = true;
    public bool AllowFriendRequests { get; set; } = true;
    public bool AllowMessages { get; set; } = true;
}