namespace UserService.DTO.User;
using Models;

public class ResponseUserDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsEmailVerified { get; set; }
    
}