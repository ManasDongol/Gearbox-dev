$entities = @("User", "Customer", "Staff", "Vehicle", "Part", "Vendor", "SalesInvoice", "PurchaseInvoice", "Appointment", "ServiceDetails", "ServiceHistory", "ServiceBill", "ServiceReview", "PartRequest", "Notification", "PurchaseInvoiceItem", "SalesInvoiceItem")

$presDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Presentation"
$controllersDir = Join-Path -Path $presDir -ChildPath "Controllers"

New-Item -ItemType Directory -Force -Path $controllersDir | Out-Null

foreach ($entity in $entities) {
    $controllerContent = @"
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;

namespace Gearbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ${entity}Controller : ControllerBase
    {
        private readonly I${entity}Service _service;

        public ${entity}Controller(I${entity}Service service)
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
        public async Task<IActionResult> Add([FromBody] ${entity}Dto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ${entity}Dto dto)
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
"@

    Set-Content -Path (Join-Path $controllersDir "${entity}Controller.cs") -Value $controllerContent -Encoding UTF8
}

Write-Host "Controllers generation completed."
