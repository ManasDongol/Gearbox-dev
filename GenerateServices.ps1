$entities = @("User", "Customer", "Staff", "Vehicle", "Part", "Vendor", "SalesInvoice", "PurchaseInvoice", "Appointment", "ServiceDetails", "ServiceHistory", "ServiceBill", "ServiceReview", "PartRequest", "Notification", "PurchaseInvoiceItem", "SalesInvoiceItem")

$appDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Application"
$dtosDir = Join-Path -Path $appDir -ChildPath "DTOs"
$interfacesDir = Join-Path -Path $appDir -ChildPath "Interfaces"
$servicesDir = Join-Path -Path $appDir -ChildPath "Services"

New-Item -ItemType Directory -Force -Path $dtosDir | Out-Null
New-Item -ItemType Directory -Force -Path $interfacesDir | Out-Null
New-Item -ItemType Directory -Force -Path $servicesDir | Out-Null

foreach ($entity in $entities) {
    # 1. Generate DTO
    $dtoContent = @"
using System;

namespace Gearbox.Application.DTOs
{
    public class ${entity}Dto
    {
        public Guid Id { get; set; }
        // Add additional properties here as needed
    }
}
"@
    Set-Content -Path (Join-Path $dtosDir "${entity}Dto.cs") -Value $dtoContent -Encoding UTF8

    # 2. Generate Interface
    $interfaceContent = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces
{
    public interface I${entity}Service
    {
        Task<IEnumerable<${entity}Dto>> GetAllAsync();
        Task<${entity}Dto> GetByIdAsync(Guid id);
        Task<${entity}Dto> AddAsync(${entity}Dto dto);
        Task UpdateAsync(Guid id, ${entity}Dto dto);
        Task DeleteAsync(Guid id);
    }
}
"@
    Set-Content -Path (Join-Path $interfacesDir "I${entity}Service.cs") -Value $interfaceContent -Encoding UTF8

    # 3. Generate Service
    $serviceContent = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Gearbox.Domain.Entities;
using Gearbox.Domain.Interfaces;

namespace Gearbox.Application.Services
{
    public class ${entity}Service : I${entity}Service
    {
        private readonly IUnitOfWork _unitOfWork;

        public ${entity}Service(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<${entity}Dto>> GetAllAsync()
        {
            var entities = await _unitOfWork.${entity}s.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<${entity}Dto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.${entity}s.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<${entity}Dto> AddAsync(${entity}Dto dto)
        {
            var entity = MapToEntity(dto);
            entity.Id = Guid.NewGuid(); // ensuring a new ID
            await _unitOfWork.${entity}s.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ${entity}Dto dto)
        {
            var entity = await _unitOfWork.${entity}s.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // entity.SomeProperty = dto.SomeProperty;
                _unitOfWork.${entity}s.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.${entity}s.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.${entity}s.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Manual Mapping Methods
        private ${entity}Dto MapToDto(${entity} entity)
        {
            if (entity == null) return null;
            return new ${entity}Dto
            {
                Id = entity.Id,
                // Map other properties here
            };
        }

        private ${entity} MapToEntity(${entity}Dto dto)
        {
            if (dto == null) return null;
            return new ${entity}
            {
                Id = dto.Id,
                // Map other properties here
            };
        }
    }
}
"@
    # Fix pluralization in _unitOfWork calls
    if ($entity -eq "ServiceHistory") {
        $serviceContent = $serviceContent -replace "_unitOfWork.ServiceHistorys", "_unitOfWork.ServiceHistories"
    }
    if ($entity -eq "Staff") {
        $serviceContent = $serviceContent -replace "_unitOfWork.Staffs", "_unitOfWork.Staffs"
    }

    Set-Content -Path (Join-Path $servicesDir "${entity}Service.cs") -Value $serviceContent -Encoding UTF8
}

Write-Host "Services generation completed."
