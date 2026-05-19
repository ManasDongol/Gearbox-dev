using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        UserManager<AppUser> _userManager;
        public CustomerRepository(ApplicationDbContext context,UserManager<AppUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<Customer?> GetCustomerWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.ServiceHistories)
                .Include(c => c.SalesServicesInvoices)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<IEnumerable<Customer>> GetHighSpendersAsync()
        {
            return await _dbSet
                .Include(c => c.SalesServicesInvoices)
                .Where(c => c.SalesServicesInvoices.Sum(i => i.TotalAmount) > 5000) // 5000 threshold based on feature list
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithPendingCreditsAsync()
        {
            return await _dbSet
                .Include(c => c.SalesServicesInvoices)
                .Where(c => c.SalesServicesInvoices.Any(i => !i.PaymentStatus))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            
            return await _dbSet
                .Include(c => c.Vehicles)
                .Where(c => c.User.FirstName.ToLower().Contains(searchTerm) || 
                            c.User.PhoneNumber.Contains(searchTerm) ||
                            c.Vehicles.Any(v => v.NumberPlate.ToLower().Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithUsersAsync()
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.SalesServicesInvoices)
                .ToListAsync();
        }
 

    
    }
}
