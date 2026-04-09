using Gearbox.Application.DTOs;
using Gearbox.Application.Services;
using Gearbox.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gearbox.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<AppUser> _userManager,RoleManager<IdentityRole<Guid>> _roleManager, TokenService _tokenService, AuthService _authService) :ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.IsSuccess) return BadRequest(result.Errors);
        return Ok(new { message = "User registered successfully" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
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
            Secure = true, // only HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });

        return Ok(new { message = "Logged in successfully" });
    }
    
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }
    
}