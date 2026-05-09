using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Gearbox.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public UserRepository(ApplicationDbContext context, UserManager<AppUser> userManager) : base(context)
        {
            _userManager = userManager;
            
        }
        
        public async Task<List<Guid>> GetAdminIdsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Select(u => u.Id).ToList();
        }

        public async Task<List<Guid>> GetStaffIdsAsync()
        {
            var staff = await _userManager.GetUsersInRoleAsync("Staff");
            return staff.Select(u => u.Id).ToList();
        }

    }
}
