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
    
    public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user;
    }

    public async Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return user;
    }

    public async Task<User?> GetUserByUserName(string username, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        return user;
    }

    public IAsyncEnumerable<User?> GetAllUsersStream(
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .AsAsyncEnumerable();
    }

  

    public async Task<User> CreateUser(User createdUser,string salt, CancellationToken cancellationToken = default)
    {
        var user = new User()
        {
            PasswordSalt = salt,
            PasswordHash = createdUser.PasswordHash,
            IsActive = true,
            Username = createdUser.Username,
            Email = createdUser.Email,
            Settings = createdUser.Settings
            
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateUser(User updatedUser, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByUserId(updatedUser.Id, cancellationToken);
        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Settings =  updatedUser.Settings;
        user.LastLogin = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> DeleteUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if(user == null) return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
        
    }

    public async Task<bool> SoftDeleteUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var user  = await GetUserByUserId(userId, cancellationToken);
        
        if(user == null) return false;
        
        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user != null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        return user != null;
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user =  await GetUserByUserId(userId, cancellationToken);
        if(user == null) return false;
        user.LastLogin = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarData, string avatarType, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByUserId(userId, cancellationToken);
        if(user == null) return false;
        user.AvatarByte =  avatarData;
        user.AvatarType = avatarType;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSettingsAsync(Guid userId, UserSettings settings, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByUserId(userId, cancellationToken);
        if (user == null) return false;
        user.Settings = settings;
        _context.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}