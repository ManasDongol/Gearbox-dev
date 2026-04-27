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
    public class SalesServicesInvoiceItemService : ISalesServicesInvoiceItemService
    {
        private readonly ISalesServicesInvoiceItemRepository _repository;
        private readonly IPartRepository _partRepository;

        public SalesServicesInvoiceItemService(ISalesServicesInvoiceItemRepository repository, IPartRepository partRepository)
        {
            _repository = repository;
            _partRepository = partRepository;
        }

        public async Task<IEnumerable<SalesServicesInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<SalesServicesInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<SalesServicesInvoiceItemDto> AddAsync(SalesServicesInvoiceItemDto dto)
        {
            var entity = MapToEntity(dto);
            
            // Reduce stock if it's a Part
            if (dto.Type == "Part" && dto.PartId.HasValue)
            {
                var part = await _partRepository.GetByIdAsync(dto.PartId.Value);
                if (part != null)
                {
                    part.StockQuantity -= dto.Quantity;
                    _partRepository.Update(part);
                }
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task UpdateAsync(Guid id, SalesServicesInvoiceItemDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Update stock if quantity or part changed
                if (entity.Type == "Part" && entity.PartId.HasValue)
                {
                    var oldPart = await _partRepository.GetByIdAsync(entity.PartId.Value);
                    if (oldPart != null)
                    {
                        oldPart.StockQuantity += entity.Quantity; // Restore old stock
                        _partRepository.Update(oldPart);
                    }
                }

                if (dto.Type == "Part" && dto.PartId.HasValue)
                {
                    var newPart = await _partRepository.GetByIdAsync(dto.PartId.Value);
                    if (newPart != null)
                    {
                        newPart.StockQuantity -= dto.Quantity; // Subtract new stock
                        _partRepository.Update(newPart);
                    }
                }

                // Assign new values from dto
                entity.PartId = dto.PartId;
                entity.ServiceDetailsId = dto.ServiceDetailsId;
                entity.Type = dto.Type;
                entity.Quantity = dto.Quantity;
                entity.UnitPrice = dto.UnitPrice;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                if (entity.Type == "Part" && entity.PartId.HasValue)
                {
                    var part = await _partRepository.GetByIdAsync(entity.PartId.Value);
                    if (part != null)
                    {
                        part.StockQuantity += entity.Quantity; // Restore stock
                        _partRepository.Update(part);
                    }
                }

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        private SalesServicesInvoiceItemDto MapToDto(SalesServicesInvoiceItem entity)
        {
            if (entity == null) return null;
            return new SalesServicesInvoiceItemDto
            {
                Id = entity.Id,
                SalesServicesInvoiceId = entity.SalesServicesInvoiceId,
                PartId = entity.PartId,
                ServiceDetailsId = entity.ServiceDetailsId,
                Type = entity.Type,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice
            };
        }

        private SalesServicesInvoiceItem MapToEntity(SalesServicesInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new SalesServicesInvoiceItem
            {
                Id = dto.Id,
                SalesServicesInvoiceId = dto.SalesServicesInvoiceId,
                PartId = dto.PartId,
                ServiceDetailsId = dto.ServiceDetailsId,
                Type = dto.Type,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };
        }
    }
}
