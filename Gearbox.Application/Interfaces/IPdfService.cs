namespace Gearbox.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateFinancialReportAsync(string period = "yearly");
    Task<byte[]> GenerateCustomerReportAsync();
    Task<byte[]> GenerateRegularCustomersReportAsync();
    Task<byte[]> GenerateHighSpendersReportAsync();
    Task<byte[]> GeneratePendingCreditsReportAsync();
}
