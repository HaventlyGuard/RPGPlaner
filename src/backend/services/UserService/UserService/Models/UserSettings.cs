namespace UserService.Models;

public class UserSettings
{
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool InGameNotifications { get; set; } = true;
    
    public string Language { get; set; } = "ru";
    public string TimeZone { get; set; } = "Europe/Moscow";
    public string DateFormat { get; set; } = "dd.MM.yyyy";
    public string TimeFormat { get; set; } = "24h";
    
    public PrivacySettings Privacy { get; set; } = new();
    
    public bool ShowTutorial { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;
    public bool MusicEnabled { get; set; } = true;
    public int SoundVolume { get; set; } = 80;
    public int MusicVolume { get; set; } = 60;
    
    public string Theme { get; set; } = "dark";
    public bool CompactMode { get; set; } = false;
    public bool AnimationsEnabled { get; set; } = true;
}