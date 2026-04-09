namespace Gearbox.Application.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Role { get; set; } = "Customer"; // "Customer", "Staff", "Admin"

    // Optional for profiles
    public string? FullName { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    
}