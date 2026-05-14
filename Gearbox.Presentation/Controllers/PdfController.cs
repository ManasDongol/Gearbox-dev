using Gearbox.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gearbox.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;

    public PdfController(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet("financial-report")]
    public async Task<IActionResult> GenerateFinancialReport()
    {
        var pdf = await _pdfService.GenerateFinancialReportAsync();
        var fileName = $"gearbox-financial-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    [HttpGet("customer-report")]
    public async Task<IActionResult> GenerateCustomerReport()
    {
        var pdf = await _pdfService.GenerateCustomerReportAsync();
        var fileName = $"gearbox-customer-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }
}
