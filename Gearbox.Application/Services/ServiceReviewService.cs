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
        private readonly IUnitOfWork _unitOfWork;

        public ServiceReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceReviewDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.ServiceReviews.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<ServiceReviewDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceReviews.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<ServiceReviewDto> AddAsync(ServiceReviewDto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.ServiceReviews.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ServiceReviewDto dto)
        {
            var entity = await _unitOfWork.ServiceReviews.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.ServiceReviews.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.ServiceReviews.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.ServiceReviews.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private ServiceReviewDto MapToDto(ServiceReview entity)
        {
            if (entity == null) return null;
            return new ServiceReviewDto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private ServiceReview MapToEntity(ServiceReviewDto dto)
        {
            if (dto == null) return null;
            return new ServiceReview
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
