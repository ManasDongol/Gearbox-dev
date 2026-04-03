using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<NotificationDto> GetByIdAsync(Guid id);
        Task<NotificationDto> AddAsync(NotificationDto dto);
        Task UpdateAsync(Guid id, NotificationDto dto);
        Task DeleteAsync(Guid id);
    }
}
