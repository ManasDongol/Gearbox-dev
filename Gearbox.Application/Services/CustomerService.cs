using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.BackgroundJobs;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Customer;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Application.Services
{
    public class CustomerService(ICustomerRepository _Repository, IUserRepository _userRepository, UserManager<AppUser> _userManager,   RoleManager<IdentityRole<Guid>> _roleManager,
        EmailQueue _emailQueue) : ICustomerService
    {
       

      

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var entities = await _Repository.GetCustomersWithUsersAsync();
            return entities.Select(c => new CustomerDto
            {
                
                UserId = c.UserId,
                UserName = c.User.UserName,
                Email = c.User.Email,
                PhoneNumber = c.User.PhoneNumber,
                Address = c.User.Address,
                TotalSpent = GetTotalSpent(c),
                PendingCredits = GetPendingCredits(c),
                FirstName = c.User.FirstName,
                LastName = c.User.LastName,
            }).ToList();
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            var entity = await _Repository.GetCustomerWithDetailsAsync(id);
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
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = false // 🔐 important
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    Console.WriteLine(error.Description);

                throw new Exception("User creation failed");
            }

            //  Ensure Customer role exists
            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Customer"));

            //  Assign role
            await _userManager.AddToRoleAsync(newUser, "Customer");

            try
            {
                var entity = MapToEntity(dto, newUser.Id);

                entity.UserId = newUser.Id;
                entity.RegisteredSince = DateTime.UtcNow;
                entity.PendingCredits = 0;
                entity.TotalSpent = 0;

                await _Repository.AddAsync(entity);
                await _Repository.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(newUser); // rollback
                throw;
            }

            //  EMAIL CONFIRMATION
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmLink =
                $"http://localhost:4200/confirm-email?userId={newUser.Id}&token={Uri.EscapeDataString(token)}";

            _emailQueue.Enqueue(new EmailJob
            {
                ToEmail = newUser.Email!,
                Subject = "Verify your Gearbox account",
                Type = EmailType.EmailVerification,
                Data = new Dictionary<string, object>
                {
                    { "link", confirmLink }
                }
            });

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
               
                TotalSpent = GetTotalSpent(entity),
                PendingCredits = GetPendingCredits(entity),
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
            
               
            };
        }

        private static decimal GetTotalSpent(Customer customer)
        {
            return customer.SalesServicesInvoices?.Sum(invoice => invoice.TotalAmount) ?? 0;
        }

        private static decimal GetPendingCredits(Customer customer)
        {
            return customer.SalesServicesInvoices?
                .Where(invoice => !invoice.PaymentStatus)
                .Sum(invoice => invoice.TotalAmount) ?? 0;
        }
        
    }
}
