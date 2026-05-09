using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartController : ControllerBase
    {
        private readonly IPartService _service;
        private readonly INotificationService _notificationService;

        public PartController(IPartService service, INotificationService notificationService)
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
        public async Task<IActionResult> Add([FromBody] NewPartDto dto)
        {
            var result = await _service.AddAsync(dto);
            await NotifyAdminsAsync($"{GetActorName()} added a new part: {result.Name}");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PartDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var part = await _service.GetByIdAsync(id);
            await _service.DeleteAsync(id);
            await NotifyAdminsAsync($"{GetActorName()} deleted part: {part?.Name ?? id.ToString()}");
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
