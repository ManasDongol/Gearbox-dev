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
        Task<IEnumerable<ServiceReviewDto>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<ServiceReviewDto>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<IEnumerable<ServiceReviewDto>> GetByServiceIdAsync(Guid serviceId);
        Task<ServiceReviewDto> AddAsync(ServiceReviewDto dto);
        Task<bool> UpdateAsync(Guid id, ServiceReviewDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
