using UserService.DTO.User;
using UserService.Models;

namespace UserService.Services.Interfaces;

public interface IUserService
{
    public Task<ResponseUserDTO?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task<ResponseUserDTO?> GetUserByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<ResponseUserDTO?> GetUserByUserNameAsync(string username, CancellationToken cancellationToken = default);
    public Task<IEnumerable<ResponseUserDTO>?> GetAllUsersAsync(CancellationToken cancellationToken = default);
    
    public Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createdUser,string salt, CancellationToken cancellationToken = default);
    public Task<UpdateUserDTO> UpdateUserAsync(UpdateUserDTO updatedUser, CancellationToken cancellationToken = default);
    
    public Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<bool> SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType, CancellationToken cancellationToken = default);
    Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings, CancellationToken cancellationToken = default);
}