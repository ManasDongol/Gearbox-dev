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
            return entities.Select(e => MapToDto(e));
        }

        public async Task<StaffDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
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
                await _repository.SaveChangesAsync();

            }
            catch (Exception e)
            { 
                await _userManager.DeleteAsync(newUser);
                throw;
            }
          
         
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, StaffDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
               
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        private StaffDto MapToDto(Staff entity)
        {
            if (entity == null) return null;
            return new StaffDto
            {
           
                UserId = entity.UserId,
                
                Department = entity.Department,
                JobTitle = entity.JobTitle,
              
            };
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
