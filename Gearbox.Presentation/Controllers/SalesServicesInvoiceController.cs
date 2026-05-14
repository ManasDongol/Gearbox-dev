using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesServicesInvoiceController : ControllerBase
    {
        private readonly ISalesServicesInvoiceService _service;

        public SalesServicesInvoiceController(ISalesServicesInvoiceService service)
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
        public async Task<IActionResult> Add([FromBody] NewSalesServicesInvoiceDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SalesServicesInvoiceDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            var result = await _service.MarkAsPaidAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SalesServicesInvoiceItemController : ControllerBase
    {
        private readonly ISalesServicesInvoiceItemService _service;

        public SalesServicesInvoiceItemController(ISalesServicesInvoiceItemService service)
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
        public async Task<IActionResult> Add([FromBody] SalesServicesInvoiceItemDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SalesServicesInvoiceItemDto dto)
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
