namespace Gearbox.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateFinancialReportAsync();
    Task<byte[]> GenerateCustomerReportAsync();
}
