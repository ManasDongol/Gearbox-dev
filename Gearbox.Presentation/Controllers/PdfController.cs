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
    public async Task<IActionResult> GenerateFinancialReport([FromQuery] string period = "yearly")
    {
        var pdf = await _pdfService.GenerateFinancialReportAsync(period);
        var fileName = $"gearbox-financial-report-{period}-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    [HttpGet("customer-report")]
    public async Task<IActionResult> GenerateCustomerReport()
    {
        var pdf = await _pdfService.GenerateCustomerReportAsync();
        var fileName = $"gearbox-customer-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    [HttpGet("regular-customers-report")]
    public async Task<IActionResult> GenerateRegularCustomersReport()
    {
        var pdf = await _pdfService.GenerateRegularCustomersReportAsync();
        var fileName = $"gearbox-regular-customers-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    [HttpGet("high-spenders-report")]
    public async Task<IActionResult> GenerateHighSpendersReport()
    {
        var pdf = await _pdfService.GenerateHighSpendersReportAsync();
        var fileName = $"gearbox-high-spenders-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    [HttpGet("pending-credits-report")]
    public async Task<IActionResult> GeneratePendingCreditsReport()
    {
        var pdf = await _pdfService.GeneratePendingCreditsReportAsync();
        var fileName = $"gearbox-pending-credits-report-{DateTime.UtcNow:yyyyMMdd-HHmm}.pdf";
        return File(pdf, "application/pdf", fileName);
    }
}
