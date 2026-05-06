using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Gearbox.Application.DTOs;
using Gearbox.Application.Services;
using Gearbox.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gearbox.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<AppUser> _userManager,
    RoleManager<IdentityRole<Guid>> _roleManager,
    TokenService _tokenService,
    AuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.IsSuccess) return BadRequest(result.Errors);
        return Ok(new { message = "User registered successfully" });
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token);

        if (!result.IsSuccess)
        {
            foreach (var e in result.Errors)
                Console.WriteLine(e);

            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost("login")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        // Set HttpOnly cookie
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // only HTTPS
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });


        return Ok(new { message = "Logged in successfully" });
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();           // "role" not ClaimTypes.Role

        return Ok(new { userId, email, roles });
    }

    [HttpGet("test")]
    public IActionResult hello()
    {
        return Ok("testings");
    }
}