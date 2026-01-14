using UserService.Models;

namespace UserService.DTO.User;

public class SettingsUserResponse
{
    public Guid UserId { get; set; }
    public UserSettings UserSettings { get; set; }
}