namespace Gearbox.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateFinancialReportAsync();
    Task<byte[]> GenerateCustomerReportAsync();
    Task<byte[]> GenerateRegularCustomersReportAsync();
    Task<byte[]> GenerateHighSpendersReportAsync();
    Task<byte[]> GeneratePendingCreditsReportAsync();
}
