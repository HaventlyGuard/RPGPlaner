using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        var users = _userService.GetAllUsersAsync();
        if (users == null) BadRequest("User not found");
        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public ActionResult<User> GetUserById(Guid id)
    {
        var user = _userService.GetUserByUserIdAsync(id);
        if (user == null) BadRequest("User not found");
        return Ok(user);
    }
    
    
    
}