using UserService.DTO.User;
using UserService.Models;

namespace UserService.Services.Interfaces;

public interface IUserService
{
    public Task<ResponseUserDTO?> GetUserByEmail(string email);
    public Task<ResponseUserDTO?> GetUserByUserId(Guid userId);
    public Task<ResponseUserDTO?> GetUserByUserName(string username);
    public Task<IQueryable<ResponseUserDTO>?> GetAllUsers();
    
    public Task<CreateUserDTO> CreateUser(CreateUserDTO createdUser);
    public Task<UpdateUserDTO> UpdateUser(UpdateUserDTO updatedUser);
    
    public Task<bool> DeleteUser(Guid userId);
    public Task<bool> SoftDeleteUser(Guid userId);
    
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> UpdateLastLoginAsync(Guid userId);
    Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType);
    Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings);
}