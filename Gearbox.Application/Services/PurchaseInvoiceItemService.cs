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
    public class PurchaseInvoiceItemService : IPurchaseInvoiceItemService
    {
        private readonly IPurchaseInvoiceItemRepository _repository;
        private readonly IPartRepository _partRepository;

        public PurchaseInvoiceItemService(IPurchaseInvoiceItemRepository repository, IPartRepository partRepository)
        {
            _repository = repository;
            _partRepository = partRepository;
        }

        public async Task<IEnumerable<PurchaseInvoiceItemDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => MapToDto(e));
        }

        public async Task<PurchaseInvoiceItemDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<NewPurchaseInvoiceItemDto> AddAsync(NewPurchaseInvoiceItemDto dto)
        {
            foreach (var item in dto._items)
            {
                var entity = MapToEntity(item);
                entity.PurchaseInvoiceId = dto.PurchaseInvoiceId;
                entity.PartId = item.PartId;
                await _repository.AddAsync(entity);
                
                // Update part stock
                var part = await _partRepository.GetByIdAsync(item.PartId);
                if (part != null)
                {
                    part.StockQuantity += item.Quantity;
                    _partRepository.Update(part);
                }
            }
            
            await _repository.SaveChangesAsync();
            return dto;
        }

        public async Task UpdateAsync(Guid id, PurchaseInvoiceItemDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Update stock if quantity or part changed
                var oldPart = await _partRepository.GetByIdAsync(entity.PartId);
                if (oldPart != null)
                {
                    oldPart.StockQuantity -= entity.Quantity;
                    _partRepository.Update(oldPart);
                }

                var newPart = await _partRepository.GetByIdAsync(dto.PartId);
                if (newPart != null)
                {
                    newPart.StockQuantity += dto.Quantity;
                    _partRepository.Update(newPart);
                }

                // Update individual properties
                entity.PartId = dto.PartId;
                entity.Quantity = dto.Quantity;
                entity.CostPrice = dto.CostPrice;
             
                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                var part = await _partRepository.GetByIdAsync(entity.PartId);
                if (part != null)
                {
                    part.StockQuantity -= entity.Quantity;
                    _partRepository.Update(part);
                }

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        private PurchaseInvoiceItemDto MapToDto(PurchaseInvoiceItem entity)
        {
            if (entity == null) return null;
            return new PurchaseInvoiceItemDto
            {
               
                PartId = entity.PartId,
                Quantity = entity.Quantity,
                CostPrice = entity.CostPrice,
            };
        }

        private PurchaseInvoiceItem MapToEntity(PurchaseInvoiceItemDto dto)
        {
            if (dto == null) return null;
            return new PurchaseInvoiceItem
            {
               
                PartId = dto.PartId,
                Quantity = dto.Quantity,
                CostPrice = dto.CostPrice,
            };
        }
    }
}
