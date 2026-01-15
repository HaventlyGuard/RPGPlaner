using UserService.Models;

namespace UserService.Repository.Intefaces;

public interface IUserRepository
{
    public Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default);
    public Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellationToken = default);
    public Task<User?> GetUserByUserName(string username, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<User?> GetAllUsersStream(CancellationToken cancellationToken = default);
    
    public Task<User> CreateUser(User createdUser,string salt, CancellationToken cancellationToken = default);
    public Task<User> UpdateUser(User updatedUser, CancellationToken cancellationToken = default);
    
    public Task<bool> DeleteUser(Guid userId,  CancellationToken cancellationToken = default);
    public Task<bool> SoftDeleteUser(Guid userId,  CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username,  CancellationToken cancellationToken = default);
    Task<bool> UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType,  CancellationToken cancellationToken = default);
    Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings,  CancellationToken cancellationToken = default);
}