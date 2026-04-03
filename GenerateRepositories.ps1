$entities = @("User", "Customer", "Staff", "Vehicle", "Part", "Vendor", "SalesInvoice", "PurchaseInvoice", "Appointment", "ServiceDetails", "ServiceHistory", "ServiceBill", "ServiceReview", "PartRequest", "Notification", "PurchaseInvoiceItem", "SalesInvoiceItem")

$domainDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Domain\Interfaces"
$infraDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Infrastructure\Repositories"

foreach ($entity in $entities) {
    # Generate Domain Interface
    $interfaceContent = @"
using Gearbox.Domain.Entities;

namespace Gearbox.Domain.Interfaces
{
    public interface I${entity}Repository : IGenericRepository<${entity}>
    {
    }
}
"@
    $interfacePath = Join-Path -Path $domainDir -ChildPath "I${entity}Repository.cs"
    Set-Content -Path $interfacePath -Value $interfaceContent -Encoding UTF8

    # Generate Infrastructure Repository
    $repoContent = @"
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;
using Gearbox.Infrastructure.Data;

namespace Gearbox.Infrastructure.Repositories
{
    public class ${entity}Repository : GenericRepository<${entity}>, I${entity}Repository
    {
        public ${entity}Repository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
"@
    $repoPath = Join-Path -Path $infraDir -ChildPath "${entity}Repository.cs"
    Set-Content -Path $repoPath -Value $repoContent -Encoding UTF8
}

Write-Host "Repositories generation completed."
