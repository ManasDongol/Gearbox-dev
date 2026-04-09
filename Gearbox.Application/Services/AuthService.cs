using Gearbox.Application.DTOs;
using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Application.Services;

public class AuthService(UserManager<AppUser> _userManager,RoleManager<IdentityRole<Guid>> _roleManager,AuthRepository _authRepo)
{
    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        Console.WriteLine(dto.Password);
        Console.WriteLine("THIS HAS BEEN CALLED");
        // Check for existing username/email
        if (await _userManager.FindByNameAsync(dto.Username) != null)
            return Result.Failure("Username already exists.");
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            return Result.Failure("Email already exists.");

        Console.WriteLine(dto.Password);
        // Create Identity user
        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            EmailConfirmed = true
        };
        
        Console.WriteLine(dto.Password);

        var identityResult = await _userManager.CreateAsync(user, dto.Password);
        if (!identityResult.Succeeded)
            return Result.Failure(identityResult.Errors.Select(e => e.Description));

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(dto.Role))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(dto.Role));

        await _userManager.AddToRoleAsync(user, dto.Role);
/*
        // Create profile
        if (dto.Role == "Customer")
        {
            var customer = new Customer
            {
                UserId = user.Id,
                FullName = dto.FullName
            };
            await _authRepo.CreateCustomerAsync(customer);
        }
        else if (dto.Role == "Staff" || dto.Role == "Admin")
        {
            var staff = new Staff
            {
                UserId = user.Id,
                FullName = dto.FullName,
                Department = dto.Department ?? "General",
                JobTitle = dto.JobTitle ?? "Staff"
            };
            await _authRepo.CreateStaffAsync(staff);
        }*/

        return Result.Success();
    }
}