using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;

namespace Gearbox.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Notifications.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<NotificationDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<NotificationDto> AddAsync(NotificationDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Notifications.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, NotificationDto dto)
        {
            var entity = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Notifications.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Notifications.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private NotificationDto MapToDto(Notification entity)
        {
            if (entity == null) return null;
            return new NotificationDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private Notification MapToEntity(NotificationDto dto)
        {
            if (dto == null) return null;
            return new Notification
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
