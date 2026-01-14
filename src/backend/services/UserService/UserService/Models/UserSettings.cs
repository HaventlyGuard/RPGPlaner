using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;


public class UserSettings
{
    public string Language { get; set; } = "ru";
    public string Theme { get; set; } = "dark";
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;
    public bool MusicEnabled { get; set; } = true;
    public int SoundVolume { get; set; } = 80;
    public int MusicVolume { get; set; } = 60;
}