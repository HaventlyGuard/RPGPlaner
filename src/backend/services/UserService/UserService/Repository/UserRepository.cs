using Microsoft.EntityFrameworkCore;
using UserService.DataAcces;
using UserService.Models;
using UserService.Repository.Intefaces;

namespace UserService.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;
    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<User?> GetUserByUserId(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }

    public async Task<User?> GetUserByUserName(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user;
    }

    public async Task<IQueryable<User>?> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users.AsQueryable();
    }

    public async Task<User> CreateUser(User createdUser)
    {
        var user = createdUser;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUser(User updatedUser)
    {
        var user = await GetUserByUserId(updatedUser.Id);
        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Settings =  updatedUser.Settings;
        user.LastLogin = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if(user == null) return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
        
    }

    public async Task<bool> SoftDeleteUser(Guid userId)
    {
        var user  = await GetUserByUserId(userId);
        
        if(user == null) return false;
        
        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user != null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user != null;
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId)
    {
        var user =  await GetUserByUserId(userId);
        if(user == null) return false;
        user.LastLogin = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType)
    {
        var user = await GetUserByUserId(userId);
        if(user == null) return false;
        user.AvatarByte =  avatarData;
        user.AvatarType = avatarType;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings)
    {
        var user = await GetUserByUserId(userId);
        if (user == null) return false;
        user.Settings = settings;
        _context.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}