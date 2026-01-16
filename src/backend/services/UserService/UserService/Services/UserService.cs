
using System.Security.Cryptography;
using UserService.DTO.MapExtensions;
using UserService.DTO.User;
using UserService.Models;
using UserService.Repository.Intefaces;
using UserService.Services.Interfaces;

namespace UserService.Services;


public class UserService : IUserService
{
    private  IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<ResponseUserDTO?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(email) ) return null;
        if (isDangerInput(email)) throw new Exception($"ваш email:{email} содержит недопустимые символы!");
        var user =  await _userRepository.GetUserByEmail(email, cancellationToken);
        return user?.UserToResponseUser();
    }

    public async Task<ResponseUserDTO?> GetUserByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if(userId == Guid.Empty) return null;
        var user = await _userRepository.GetUserByUserId(userId, cancellationToken);
        return  user?.UserToResponseUser();
    }

    public async Task<ResponseUserDTO?> GetUserByUserNameAsync(string username, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(username) || isDangerInput(username)) return null;
        var user = await _userRepository.GetUserByUserName(username, cancellationToken);
        return user?.UserToResponseUser();
    }

    public async Task<IEnumerable<ResponseUserDTO>?> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var usersStream = _userRepository.GetAllUsersStream(cancellationToken);
    
        var usersList = new List<User>();
    
        await foreach (var user in usersStream)
        {
            if (user != null)
                usersList.Add(user);
        }
    
        return usersList.Select(u=> u.UserToResponseUser());
    }

    public async Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createdUser,string salt, CancellationToken cancellationToken = default)
    {
        if(createdUser == null || isDangerInput(createdUser.Email) || isDangerInput(createdUser.Username)) return null;
        var PasswordHashAndSalt= PasswordHashSaltGenerator(password: createdUser.Password);
        createdUser.Password = PasswordHashAndSalt.salt +  PasswordHashAndSalt.hash;
        var user = await _userRepository.CreateUser(createdUser.CreateToUser(),PasswordHashAndSalt.salt, cancellationToken);
        return user.UserToCreateUser();
    }

    public async Task<UpdateUserDTO> UpdateUserAsync(UpdateUserDTO updatedUser, CancellationToken cancellationToken = default)
    {
        if(updatedUser == null || isDangerInput(updatedUser.Email) || isDangerInput(updatedUser.Username)) return null;
        var user = await _userRepository.UpdateUser(updatedUser.UpdateToUser(), cancellationToken);
        return user.UserToUpdateUser();
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByUserId(userId, cancellationToken);
        if(user == null) return false;
        return await _userRepository.DeleteUser(userId, cancellationToken);;
    }

    public async Task<bool> SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByUserId(userId, cancellationToken);
        if(user == null) return false;
        return await _userRepository.SoftDeleteUser(userId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(email) || isDangerInput(email)) return false;
        return await _userRepository.ExistsByEmailAsync(email, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(username) || isDangerInput(username)) return false;
        return await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByUserId(userId, cancellationToken);
        if(user == null) return false;
        user.LastLogin = DateTime.UtcNow;
        return true;
    }

    public async Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType, CancellationToken cancellationToken = default)
    {
        return await _userRepository.UpdateAvatarAsync(userId, avatarData, avatarType, cancellationToken);
    }

    public async Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings, CancellationToken cancellationToken = default)
    {
        return await _userRepository.UpdateSettingsAsync(userId, settings, cancellationToken);
    }
    
    public static (string hash, string salt)PasswordHashSaltGenerator(string password, CancellationToken cancellationToken = default)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );
        
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static List<string> dangerInputs =
    [
        "select", "insert", "delete", "exec", "execute", "update", "create"
    ];
    
    public static bool isDangerInput(string input)
    {
        return dangerInputs.Contains(input.ToLower());
    }
}