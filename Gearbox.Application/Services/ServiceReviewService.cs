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
    public class ServiceReviewService : IServiceReviewService
    {
        private readonly IServiceReviewRepository _repository;

        public ServiceReviewService(IServiceReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ServiceReviewDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceReviewDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceReviewDto> AddAsync(ServiceReviewDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceReviewDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
              
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        private ServiceReviewDto MapToDto(ServiceReview entity)
        {
            if (entity == null) return null;
            return new ServiceReviewDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                AppointmentId = entity.AppointmentId,
                Rating = entity.Rating,
                Comment = entity.Comment,
                ReviewDate = entity.ReviewDate,
              
            };
        }

        private ServiceReview MapToEntity(ServiceReviewDto dto)
        {
            if (dto == null) return null;
            return new ServiceReview
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                AppointmentId = dto.AppointmentId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = dto.ReviewDate,
                
            };
        }
    }
}
