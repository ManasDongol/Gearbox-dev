using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface IServiceReviewService
    {
        Task<IEnumerable<ServiceReviewDto>> GetAllAsync();
        Task<ServiceReviewDto> GetByIdAsync(Guid id);
        Task<ServiceReviewDto> AddAsync(ServiceReviewDto dto);
        Task UpdateAsync(Guid id, ServiceReviewDto dto);
        Task DeleteAsync(Guid id);
    }
}
