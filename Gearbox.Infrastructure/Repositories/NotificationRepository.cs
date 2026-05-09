using System.Security.Claims;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public NotificationRepository(
            ApplicationDbContext context,
            UserManager<AppUser> userManager
        ) : base(context)
        {
            _userManager = userManager;
        }
        
        public async Task AddRangeAsync(IEnumerable<Notification> entities)
        {
            await _context.Notifications.AddRangeAsync(entities);
        }
        public async Task<IEnumerable<Notification>> GetRecentNotifications(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Enumerable.Empty<Notification>();

            var roles = await _userManager.GetRolesAsync(user);

            IQueryable<Notification> query = _context.Notifications;

            if (roles.Contains("Admin"))
            {
                query = query.Where(n =>
                    n.TargetRole == "Admin" ||
                    n.IsGlobal);
            }
            else if (roles.Contains("Staff"))
            {
                query = query.Where(n =>
                    n.TargetRole == "Staff");
            }
            else
            {
                query = query.Where(n =>
                    n.UserId == Guid.Parse(userId));
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();
        }
    }
}
