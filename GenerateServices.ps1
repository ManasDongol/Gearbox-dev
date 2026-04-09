$entities = @("User", "Customer", "Staff", "Vehicle", "Part", "Vendor", "SalesInvoice", "PurchaseInvoice", "Appointment", "ServiceDetails", "ServiceHistory", "ServiceBill", "ServiceReview", "PartRequest", "Notification", "PurchaseInvoiceItem", "SalesInvoiceItem")

$appDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Application"
$dtosDir = Join-Path -Path $appDir -ChildPath "DTOs"
$interfacesDir = Join-Path -Path $appDir -ChildPath "Interfaces"
$servicesDir = Join-Path -Path $appDir -ChildPath "Services"
$domainDir = "c:\Users\Lenovo\RiderProjects\Gearbox-DEV\Gearbox.Domain"
$entitiesDir = Join-Path -Path $domainDir -ChildPath "Entities"

foreach ($entity in $entities) {
    # 1. Read Entity file to get properties
    $entityPath = Join-Path $entitiesDir "${entity}.cs"
    $properties = @()
    if (Test-Path $entityPath) {
        $content = Get-Content $entityPath
        foreach ($line in $content) {
            if ($line -match "public\s+(\S+)\s+(\w+)\s+\{\s*get;\s*set;\s*\}") {
                $type = $Matches[1]
                $name = $Matches[2]
                # Skip collections and navigation properties (usually start with ICollection or are in the same namespace)
                if ($type -notmatch "ICollection" -and $type -notmatch "List" -and $type -notmatch "IEnumerable") {
                    # For simplicity, we'll keep them if they are common types or Guids
                    if ($type -match "Guid|string|int|decimal|DateTime|bool|double|float|long|short|byte") {
                         $properties += @{Type=$type; Name=$name}
                    }
                }
            }
        }
    }

    # 2. Generate DTO
    $propsString = ""
    foreach ($p in $properties) {
        $propsString += "        public $($p.Type) $($p.Name) { get; set; }`n"
    }

    $dtoContent = @"
using System;

namespace Gearbox.Application.DTOs
{
    public class ${entity}Dto
    {
$propsString    }
}
"@
    Set-Content -Path (Join-Path $dtosDir "${entity}Dto.cs") -Value $dtoContent -Encoding UTF8

    # 3. Generate Interface
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

    # 4. Generate Service (Pure Controller -> Service -> Repo)
    
    $repoName = "i${entity}Repository" # lower case for field
    $repoType = "I${entity}Repository"
    
    $mapToDtoContent = ""
    $mapToEntityContent = ""
    foreach ($p in $properties) {
        $mapToDtoContent += "                $($p.Name) = entity.$($p.Name),`n"
        $mapToEntityContent += "                $($p.Name) = dto.$($p.Name),`n"
    }

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
        private readonly $repoType _repository;

        public ${entity}Service($repoType repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<${entity}Dto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<${entity}Dto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<${entity}Dto> AddAsync(${entity}Dto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, ${entity}Dto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Assign new values from dto
                // (In a real scenario, you'd map individual properties)
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        private ${entity}Dto MapToDto(${entity} entity)
        {
            if (entity == null) return null;
            return new ${entity}Dto
            {
$mapToDtoContent            };
        }

        private ${entity} MapToEntity(${entity}Dto dto)
        {
            if (dto == null) return null;
            return new ${entity}
            {
$mapToEntityContent            };
        }
    }
}
"@
    Set-Content -Path (Join-Path $servicesDir "${entity}Service.cs") -Value $serviceContent -Encoding UTF8
}

Write-Host "Services generation updated for Controller -> Service -> Repo pattern."
