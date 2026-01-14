using UserService.Models;

namespace UserService.Repository.Intefaces;

public interface IUserRepository
{
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserByUserId(Guid userId);
    public Task<User?> GetUserByUserName(string username);
    public Task<IQueryable<User>?> GetAllUsers();
    
    public Task<User> CreateUser(User createdUser);
    public Task<User> UpdateUser(User updatedUser);
    
    public Task<bool> DeleteUser(Guid userId);
    public Task<bool> SoftDeleteUser(Guid userId);
    
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> UpdateLastLoginAsync(Guid userId);
    Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType);
    Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings);
}