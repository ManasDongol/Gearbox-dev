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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Users.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<UserDto> AddAsync(UserDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.Users.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, UserDto dto)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.Users.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Users.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private UserDto MapToDto(User entity)
        {
            if (entity == null) return null;
            return new UserDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private User MapToEntity(UserDto dto)
        {
            if (dto == null) return null;
            return new User
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
