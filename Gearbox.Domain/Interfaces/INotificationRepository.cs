using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        public Task AddRangeAsync(IEnumerable<Notification> entities);
        public Task<IEnumerable<Notification>> GetRecentNotifications(string userid);
    }
}
