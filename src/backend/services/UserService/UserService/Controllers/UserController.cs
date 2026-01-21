using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using UserService.DTO.User;
using UserService.Models;
using UserService.Repository.Intefaces;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<ResponseUserDTO>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        if (users == null) BadRequest("User not found");
        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async  Task<ActionResult<User>> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserByUserIdAsync(id, cancellationToken);
        if (user == null) BadRequest("User not found");
        return Ok(user);
    }

    [HttpGet("users/email/{email}")]
    public async Task<ActionResult<ResponseUserDTO>> GetUserByUsername(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserByEmailAsync(email, cancellationToken);
        if (user == null) BadRequest("User not found");
        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<ActionResult<ResponseUserDTO>> CreateUser([FromBody] CreateUserDTO createdUser, CancellationToken cancellationToken = default)
    {
        var user = await _userService.CreateUserAsync(createdUser, cancellationToken);
        if (user == null)  BadRequest("User not found");
        return Ok(user);
    }
    
    
    
}