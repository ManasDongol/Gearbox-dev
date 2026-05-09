using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetCustomerWithDetailsAsync(Guid id);
        
        Task<IEnumerable<Customer>> GetHighSpendersAsync();
        Task<IEnumerable<Customer>> GetCustomersWithPendingCreditsAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<Customer>> GetCustomersWithUsersAsync();


      
    }
}
