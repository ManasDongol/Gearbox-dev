using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.DTOs.Staff;
using Gearbox.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly INotificationService _notificationService;

        public StaffController(IStaffService service, INotificationService notificationService)
        {
            _service = service;
            _notificationService = notificationService;
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] NewStaffDto dto)
        {
            var result = await _service.AddAsync(dto);
            await NotifyAdminsAsync($"{GetActorName()} added staff: {dto.FirstName} {dto.LastName}");
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StaffDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var staff = await _service.GetByIdAsync(id);
            try
            {
                await _service.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            var staffName = staff == null ? id.ToString() : $"{staff.FirstName} {staff.LastName}".Trim();
            await NotifyAdminsAsync($"{GetActorName()} deleted staff: {staffName}");
            return NoContent();
        }

        [HttpPost("{id}/promote-admin")]
        public async Task<IActionResult> PromoteToAdmin(Guid id)
        {
            try
            {
                await _service.PromoteToAdminAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var staff = await _service.GetByIdAsync(id);
            var staffName = staff == null ? id.ToString() : $"{staff.FirstName} {staff.LastName}".Trim();
            await NotifyAdminsAsync($"{GetActorName()} promoted staff to admin: {staffName}");
            return NoContent();
        }

        private async Task NotifyAdminsAsync(string message)
        {
            if (User.Identity?.IsAuthenticated != true)
                return;

            var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(actorId, out var parsedActorId))
            {
                await _notificationService.BroadcastAdminsAsync(message, parsedActorId);
            }
        }

        private string GetActorName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "A staff member";
        }
    }
}
