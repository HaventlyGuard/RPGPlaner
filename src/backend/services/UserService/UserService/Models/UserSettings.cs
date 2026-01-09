using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

// ComplexType - будет сохранен как JSONB в User
[ComplexType]
public class UserSettings
{
    [Column("email_notifications")]
    public bool EmailNotifications { get; set; } = true;

    [Column("push_notifications")]
    public bool PushNotifications { get; set; } = true;

    [Column("in_game_notifications")]
    public bool InGameNotifications { get; set; } = true;

    [Column("language")]
    public string Language { get; set; } = "ru";

    [Column("timezone")]
    public string TimeZone { get; set; } = "Europe/Moscow";

    [Column("date_format")]
    public string DateFormat { get; set; } = "dd.MM.yyyy";

    [Column("time_format")]
    public string TimeFormat { get; set; } = "24h";

    [Column("show_tutorial")]
    public bool ShowTutorial { get; set; } = true;

    [Column("sound_enabled")]
    public bool SoundEnabled { get; set; } = true;

    [Column("music_enabled")]
    public bool MusicEnabled { get; set; } = true;

    [Column("sound_volume")]
    public int SoundVolume { get; set; } = 80;

    [Column("music_volume")]
    public int MusicVolume { get; set; } = 60;

    [Column("theme")]
    public string Theme { get; set; } = "dark";

    [Column("compact_mode")]
    public bool CompactMode { get; set; } = false;

    [Column("animations_enabled")]
    public bool AnimationsEnabled { get; set; } = true;
}