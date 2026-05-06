using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories;

public class AuthRepository(ApplicationDbContext _context)
{
    public async Task CreateCustomerAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task CreateStaffAsync(Staff staff)
    {
        _context.Staffs.Add(staff);
        await _context.SaveChangesAsync();
    }

    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}