using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Staff;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;
        private readonly UserManager<AppUser> _userManager;
        

        public StaffService(IStaffRepository repository,UserManager<AppUser> _usermanager)
        {
            _repository = repository;
            _userManager = _usermanager;
        }

        public async Task<IEnumerable<StaffDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var staffDtos = new List<StaffDto>();

            foreach (var entity in entities)
            {
                var dto = await MapToDtoAsync(entity);
                if (dto != null)
                {
                    staffDtos.Add(dto);
                }
            }

            var representedUserIds = staffDtos.Select(s => s.UserId).ToHashSet();
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in adminUsers.Where(a => !representedUserIds.Contains(a.Id)))
            {
                staffDtos.Add(new StaffDto
                {
                    UserId = admin.Id,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Department = "Admin",
                    JobTitle = "Administrator",
                    Role = "Admin"
                });
            }

            return staffDtos;
        }

        public async Task<StaffDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null) return await MapToDtoAsync(entity);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin")) return null;

            return new StaffDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Department = "Admin",
                JobTitle = "Administrator",
                Role = "Admin"
            };
        }

        public async Task<StaffDto> AddAsync(NewStaffDto dto)
        {
            var entity = MapToEntity(dto);

            var newUser = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Address = dto.Address,
                Email = dto.email,
                PhoneNumber = dto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                   
                }
                throw new Exception("User creation failed");
            }

            try
            {
                entity.UserId = newUser.Id;
                await _repository.AddAsync(entity);
                await _userManager.AddToRoleAsync(newUser, "Staff");
                await _repository.SaveChangesAsync();

            }
            catch (Exception e)
            { 
                await _userManager.DeleteAsync(newUser);
                throw;
            }
          
         
            return await MapToDtoAsync(entity);
        }

        public async Task UpdateAsync(Guid id, StaffDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    user.FirstName = dto.FirstName;
                    user.LastName = dto.LastName;
                    await _userManager.UpdateAsync(user);
                }

                entity.Department = dto.Department;
                entity.JobTitle = dto.JobTitle;
               
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                throw new InvalidOperationException("Admins cannot be removed from staff management.");
            }

            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task PromoteToAdminAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("Staff user was not found.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            if (await _userManager.IsInRoleAsync(user, "Staff"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Staff");
            }
        }

        private async Task<StaffDto> MapToDtoAsync(Staff entity)
        {
            if (entity == null) return null;
            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());
            return new StaffDto
            {
           
                UserId = entity.UserId,
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                Department = entity.Department,
                JobTitle = entity.JobTitle,
                Role = user == null ? "Staff" : await GetDisplayRoleAsync(user),
              
            };
        }

        private async Task<string> GetDisplayRoleAsync(AppUser user)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin")) return "Admin";
            if (await _userManager.IsInRoleAsync(user, "Staff")) return "Staff";
            return "Unassigned";
        }

        private Staff MapToEntity(StaffDto dto)
        {
            if (dto == null) return null;
            return new Staff
            {
             
                UserId = dto.UserId,
            
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                HireDate = DateTime.UtcNow
            };
        }
        
        private Staff MapToEntity(NewStaffDto dto)
        {
            if (dto == null) return null;
            return new Staff
            {
                
              
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                HireDate = DateTime.UtcNow
            };
        }
    }
}
