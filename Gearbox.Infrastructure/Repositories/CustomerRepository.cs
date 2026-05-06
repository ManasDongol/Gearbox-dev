using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Vehicles)
                .Include(c => c.ServiceHistories)
                .Include(c => c.SalesServicesInvoices)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<IEnumerable<Customer>> GetHighSpendersAsync()
        {
            return await _dbSet
                .Where(c => c.TotalSpent > 5000) // 5000 threshold based on feature list
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithPendingCreditsAsync()
        {
            return await _dbSet
                .Where(c => c.PendingCredits > 0)
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
                .ToListAsync();
        }
        

    
    }
}
