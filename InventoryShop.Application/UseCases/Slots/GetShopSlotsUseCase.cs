using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Slots;

public sealed class GetShopSlotsUseCase(
   IShopSlotsRepository shopSlotsRepository,
   EnrichedSlotDetailsFactory slotDetailsFactory,
   ILogger logger)
{
   public async Task<Result<EnrichedShopSlotDetails, Error>> GetSlotsByIdAsync(Guid id, CancellationToken ct)
   {
      ShopSlotEntity? shopSlot = await shopSlotsRepository.GetSlotById(id, ct);

      if (shopSlot is null)
      {
         logger.LogError("Can't find shop slot {ID}", id);
         return ShopSlotsErrors.ShopSlotWithIdNotFoundError(id);
      }

      return Result.Success<EnrichedShopSlotDetails, Error>(await slotDetailsFactory.CreateAsync(shopSlot, ct));
   }

   public async Task<Result<List<EnrichedShopSlotDetails>, Error>> GetAllSlotsAsync(CancellationToken ct)
   {
      var shopSlots = await shopSlotsRepository.GetAllSlotsAsync(ct);
      return await slotDetailsFactory.CreateManyAsync(shopSlots, ct);
   }

   public async Task<Result<List<EnrichedShopSlotDetails>, Error>> GetAllSlotsCreatedByPlayerAsync(Guid? creatorId, CancellationToken ct)
   {
      var s = new SlotCreatedBySpecification(creatorId);
      var shopSlots = await shopSlotsRepository.GetSlotsSpecifiedAsync(s, ct);
      return await slotDetailsFactory.CreateManyAsync(shopSlots, ct);
   }
}