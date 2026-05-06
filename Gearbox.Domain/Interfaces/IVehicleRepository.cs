using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        
        Task<IEnumerable<Vehicle>> GetCustomerVehicles(Guid customerId);
    }
}
