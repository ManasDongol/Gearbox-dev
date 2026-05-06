using Gearbox.Application.BackgroundJobs;
using Gearbox.Application.DTOs;
using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Application.Services;

public class AuthService(UserManager<AppUser> _userManager,RoleManager<IdentityRole<Guid>> _roleManager,AuthRepository _authRepo, EmailQueue _emailQueue,
    EmailTemplateService _templateService)
{
    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByNameAsync(dto.Username) != null)
            return Result.Failure("Username already exists.");

        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            return Result.Failure("Email already exists.");
        try
        {


            var user = new AppUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(user, dto.Password);

            if (!identityResult.Succeeded)
                return Result.Failure(identityResult.Errors.Select(e => e.Description));

            // role handling
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);

            //  EMAIL CONFIRMATION
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmLink =
                $"https://localhost:5001/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            _emailQueue.Enqueue(new EmailJob
            {
                ToEmail = user.Email!,
                Subject = "Verify your Gearbox account",
                Type = EmailType.EmailVerification,
                Data = new Dictionary<string, object>
                {
                    { "link", confirmLink }
                }
            });

            return Result.Success("done");
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result.Failure(ex.Message);
        }
    }
    
    public async Task<Result> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Failure("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success("Email confirmed successfully.");
    }
}