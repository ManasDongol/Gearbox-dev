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
        private readonly INotificationRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationSender _sender;

        public NotificationService(
            INotificationSender sender,
            INotificationRepository repository,
            IUserRepository userRepository)
        {
            _sender = sender;
            _repository = repository;
            _userRepository = userRepository;
        }

        // =========================
        // READ
        // =========================

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<NotificationDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<NotificationDto>> GetRecentNotifications(string userId)
        {
            var entities = await _repository.GetRecentNotifications(userId);
            return entities.Select(MapToDto);
        }

        // =========================
        // CREATE (manual insert)
        // =========================

        public async Task<NotificationDto> AddAsync(NotificationDto dto)
        {
            var entity = MapToEntity(dto);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return MapToDto(entity);
        }

        // =========================
        // USER NOTIFICATION
        // =========================

        public async Task SendToUserAsync(string userId, string message)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new Exception("Invalid userId");

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = parsedUserId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            await _sender.SendToUser(userId, MapToDto(notification));
        }

        // =========================
        // ROLE NOTIFICATION
        // =========================

        public async Task SendToRoleAsync(string role, string message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = null,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                TargetRole = role
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            await _sender.SendToRole(role, MapToDto(notification));
        }

        // =========================
        // GLOBAL NOTIFICATION
        // =========================

        public async Task BroadcastAsync(string message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = null,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                IsGlobal = true
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            await _sender.Broadcast(MapToDto(notification));
        }

        // =========================
        // ADMIN BROADCAST
        // =========================

        public async Task BroadcastAdminsAsync(string message, Guid actorUserId)
        {
            var adminIds = await _userRepository.GetAdminIdsAsync();

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = null, // IMPORTANT FIX
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                TargetRole = "Admin"
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            var dto = MapToDto(notification);

            foreach (var adminId in adminIds)
            {
                await _sender.SendToUser(adminId.ToString(), dto);
            }
        }

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateAsync(Guid id, NotificationDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null) return;

            entity.Message = dto.Message;
            entity.IsRead = dto.IsRead;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        // =========================
        // DELETE
        // =========================

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null) return;

            _repository.Remove(entity);
            await _repository.SaveChangesAsync();
        }

        // =========================
        // MAPPERS
        // =========================

        private NotificationDto MapToDto(Notification entity)
        {
            if (entity == null) return null;

            return new NotificationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Message = entity.Message,
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt,
                TargetRole = entity.TargetRole,
                IsGlobal = entity.IsGlobal
            };
        }

        private Notification MapToEntity(NotificationDto dto)
        {
            if (dto == null) return null;

            return new Notification
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                UserId = dto.UserId == Guid.Empty ? null : dto.UserId,
                Message = dto.Message,
                IsRead = dto.IsRead,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                TargetRole = dto.TargetRole,
                IsGlobal = dto.IsGlobal
            };
        }
    }
}