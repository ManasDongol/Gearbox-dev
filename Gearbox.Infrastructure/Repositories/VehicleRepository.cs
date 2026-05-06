using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {

        private ApplicationDbContext context;

        public VehicleRepository(ApplicationDbContext _context): base(_context)
        {
            context=_context;
        }
        
        public async Task<IEnumerable<Vehicle>> GetCustomerVehicles(Guid customerId)
        {
                return  context.Vehicles.Where(v => v.CustomerId == customerId);
        }
    }
}
