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

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        var users = _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
}