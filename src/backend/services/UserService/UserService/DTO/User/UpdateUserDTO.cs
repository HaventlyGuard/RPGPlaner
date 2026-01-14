namespace UserService.DTO.User;

public class UpdateUserDTO
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
}