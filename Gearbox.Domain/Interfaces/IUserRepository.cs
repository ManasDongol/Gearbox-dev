using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        public Task<List<Guid>> GetAdminIdsAsync();


        public Task<List<Guid>> GetStaffIdsAsync();
        
        
    }
}
