using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceReviewController : ControllerBase
    {
        private readonly IServiceReviewService _service;

        public ServiceReviewController(IServiceReviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var result = await _service.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointmentId(Guid appointmentId)
        {
            var result = await _service.GetByAppointmentIdAsync(appointmentId);
            return Ok(result);
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetByServiceId(Guid serviceId)
        {
            var result = await _service.GetByServiceIdAsync(serviceId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ServiceReviewDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceReviewDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
