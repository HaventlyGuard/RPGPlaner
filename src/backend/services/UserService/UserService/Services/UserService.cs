using UserService.DTO.MapExtensions;
using UserService.DTO.User;
using UserService.Models;
using UserService.Repository.Intefaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<ResponseUserDTO?> GetUserByEmail(string email)
    {
        var user =  await _userRepository.GetUserByEmail(email);
        return user.UserToResponseUser();
    }

    public async Task<ResponseUserDTO?> GetUserByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseUserDTO?> GetUserByUserName(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<IQueryable<ResponseUserDTO>?> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public async Task<CreateUserDTO> CreateUser(CreateUserDTO createdUser)
    {
        throw new NotImplementedException();
    }

    public async Task<UpdateUserDTO> UpdateUser(UpdateUserDTO updatedUser)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SoftDeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings)
    {
        throw new NotImplementedException();
    }
}