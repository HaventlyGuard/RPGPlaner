
namespace UserService.DTO.MapExtensions;
using UserService.DTO.User;
using Models;

public static class UserMapExtensions
{
    public static User CreateToUser(this CreateUserDTO createDto)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = createDto.Email,
            PasswordHash = createDto.Password, 
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User ResponseToUser(this ResponseUserDTO responseUser)
    {
        return new User
        {
            Id = responseUser.Id,
            Email = responseUser.Email,
            Username = responseUser.Username,
            PasswordHash = responseUser.PasswordHash,
            IsActive = responseUser.IsActive,
            IsAdmin = responseUser.IsAdmin,
            IsEmailVerified = responseUser.IsEmailVerified,
            FirstName = responseUser.FirstName,
            LastName = responseUser.LastName,
        };
    }

    public static User UpdateToUser(this UpdateUserDTO updateUser)
    {
        return new User
        {
            Id = updateUser.Id,
            Email = updateUser.Email,
            Username = updateUser.Username,
            PasswordHash = updateUser.PasswordHash,
            FirstName = updateUser.Firstname,
            LastName = updateUser.Lastname,
        };
    }

    public static ResponseUserDTO UserToResponseUser(this User responseUser)
    {
        return new ResponseUserDTO
        {
            Id = responseUser.Id,
            Email = responseUser.Email,
            Username = responseUser.Username,
            PasswordHash = responseUser.PasswordHash,
            IsActive = responseUser.IsActive,
            IsAdmin = responseUser.IsAdmin,
            IsEmailVerified = responseUser.IsEmailVerified,
            FirstName = responseUser.FirstName,
            LastName = responseUser.LastName,

        };
    }

    public static CreateUserDTO UserToCreateUser(this User responseUser)
    {
        return new CreateUserDTO
        {
            Email = responseUser.Email,
            Username = responseUser.Username,
            Password = responseUser.PasswordHash,

        };
    }

    public static UpdateUserDTO UserToUpdateUser(this User responseUser)
    {
        return new UpdateUserDTO
        {
            Email = responseUser.Email,
            Username = responseUser.Username,
            PasswordHash = responseUser.PasswordHash,
            Lastname = responseUser.LastName,
            Firstname = responseUser.FirstName,

        };
    }

}