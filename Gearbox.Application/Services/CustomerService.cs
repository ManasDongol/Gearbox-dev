using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Customer;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Application.Services
{
    public class CustomerService(ICustomerRepository _Repository, IUserRepository _userRepository, UserManager<AppUser> _userManager) : ICustomerService
    {
       

      

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var entities = await _Repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            var entity = await _Repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<NewCustomerDto> AddAsync(NewCustomerDto dto)
        {
        
            var newUser = new AppUser
            {
               
               FirstName = dto.FirstName,
               LastName = dto.LastName,
               Address = dto.Address,
                UserName = dto.UserName,
                Email = dto.Email,
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
                
               
                var entity = MapToEntity(dto, newUser.Id);
       
               
               entity.UserId = newUser.Id;
                entity.RegisteredSince = DateTime.UtcNow;

                await _Repository.AddAsync(entity);
                await _Repository.SaveChangesAsync();
            }
            catch
            {
                // rollback user if customer fails
                await _userManager.DeleteAsync(newUser);
                throw;
            }

            return dto;
        }

        public async Task UpdateAsync(Guid id, CustomerDto dto)
        {
            var entity = await _Repository.GetByIdAsync(id);
            if (entity != null)
            {
                
                _Repository.Update(entity);
                await _Repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid userid)
        {
            var entity = await _userManager.FindByIdAsync(userid.ToString());
            if (entity != null)
            {
               await  _userManager.DeleteAsync(entity);
            }
        }

        private CustomerDto MapToDto(Customer entity)
        {
            if (entity == null) return null;
            return new CustomerDto
            {
              
                UserId = entity.UserId,
               
                TotalSpent = entity.TotalSpent,
                PendingCredits = entity.PendingCredits,
                RegisteredSince = entity.RegisteredSince,
            };
        }

        private Customer MapToEntity(CustomerDto dto)
        {
            if (dto == null) return null;
            return new Customer
            {
          
                UserId = dto.UserId,
               
                TotalSpent = dto.TotalSpent,
                PendingCredits = dto.PendingCredits,
                RegisteredSince = dto.RegisteredSince,
            };
        }
        
        private Customer MapToEntity(NewCustomerDto dto,Guid id)
        {
            if (dto == null) return null;
            return new Customer
            {
                UserId = id,
               
                TotalSpent = dto.TotalSpent,
                PendingCredits = dto.PendingCredits,
               
            };
        }
        
    }
}
