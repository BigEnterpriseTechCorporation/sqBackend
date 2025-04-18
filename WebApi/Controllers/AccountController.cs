using System.Security.Claims;
using ApplicationCore.DTOs.Requests;
using ApplicationCore.DTOs.Responses;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;


    public AccountController(IUserService userService, ITokenService tokenService, IUserRepository userRepository)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest rq)
    {
        var user = new User
        {
            UserName = rq.UserName.ToLower(),
            Email = $"{rq.UserName}@localhost",
            FullName = rq.FullName,
        };
        
        try
        {
            var created_user = await _userService.RegisterUserAsync(user, rq.Password);
            var rs = new RegisterResponse
            {
                Id = created_user.Id,
                Role = created_user.Role,
                Token = _tokenService.GenerateJwtToken(created_user)
            };
            return CreatedAtAction(null, null, rs);
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex.Message);
            return BadRequest();
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest rq)
    {
        var isValid = _userService.ValidateCredentialsAsync(rq.username, rq.password);
        if (!await isValid)
           return Unauthorized();
        var user = await _userRepository.GetByUsernameAsync(rq.username);
        if (user == null) return Unauthorized();
        /*return Ok(new
        {
            Token = _tokenService.GenerateJwtToken(user)
        });*/
        return Ok(new LoginResponse
        {
            Id = user.Id,
            Role = user.Role,
            Token = _tokenService.GenerateJwtToken(user)
        });
    }

    [HttpGet("refresh")]
    [Authorize]
    public async Task<IActionResult> GetRefreshToken()
    {
        var claims = HttpContext.User.Claims;
        var idStr = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (idStr == null) return BadRequest();
        Guid.TryParseExact(idStr, "D", out var id);
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return Unauthorized();
        return Ok(new RefreshResponse
        {
            Token = _tokenService.GenerateJwtToken(user)
        });
    }

    [HttpGet("self")]
    [Authorize]
    public async Task<IActionResult> GetSelf()
    {
        var claims = HttpContext.User.Claims;
        var idStr = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (idStr == null) return BadRequest();
        Guid.TryParseExact(idStr, "D", out var id);
        var user = await _userRepository.GetByIdAsync(id);
        return Ok(user);
    }
}