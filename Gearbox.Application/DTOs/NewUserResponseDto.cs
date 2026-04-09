using System;

namespace Gearbox.Application.DTOs;

public class NewUserResponseDto
{
    public Guid Id { get; set; }
        
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    // Add additional properties here a
}