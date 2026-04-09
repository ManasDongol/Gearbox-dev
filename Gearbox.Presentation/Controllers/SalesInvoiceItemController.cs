using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesInvoiceItemController : ControllerBase
    {
        private readonly ISalesInvoiceItemService _service;

        public SalesInvoiceItemController(ISalesInvoiceItemService service)
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SalesInvoiceItemDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SalesInvoiceItemDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
