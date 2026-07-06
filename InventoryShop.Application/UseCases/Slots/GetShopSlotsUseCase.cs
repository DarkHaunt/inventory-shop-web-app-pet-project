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
   public async Task<Result<EnrichedShopSlotDetails, Error>> GetSlotById(Guid id, CancellationToken ct)
   {
      ShopSlotEntity? shopSlot = await shopSlotsRepository.GetSlotById(id, ct);

      if (shopSlot is null)
      {
         logger.LogError("Can't find shop slot {ID}", id);
         return ShopSlotsErrors.ShopSlotWithIdNotFoundError(id);
      }

      var result = await slotDetailsFactory.CreateAsync(shopSlot, ct);

      if (result.IsFailure)
         return result.Error;

      return Result.Success<EnrichedShopSlotDetails, Error>(result.Value);
   }

   public async Task<Result<List<EnrichedShopSlotDetails>, Error>> GetAllSlotsAsync(CancellationToken ct)
   {
      var shopSlots = await shopSlotsRepository.GetAllSlotsAsync(ct);
      return await CreateManyAsync(shopSlots, ct);
   }

   public async Task<Result<List<EnrichedShopSlotDetails>, Error>> GetAllSlotsCreatedByPlayerAsync(Guid creatorId, CancellationToken ct)
   {
      var s = new SlotCreatedBySpecification(creatorId);
      var shopSlots = await shopSlotsRepository.GetSlotsSpecifiedAsync(s, ct);
      return await CreateManyAsync(shopSlots, ct);
   }

   private async Task<Result<List<EnrichedShopSlotDetails>, Error>> CreateManyAsync(List<ShopSlotEntity> slots, CancellationToken ct)
   {
      var results = await slotDetailsFactory.CreateManyAsync(slots, ct);
      var list = new List<EnrichedShopSlotDetails>(results.Length);

      foreach (var result in results)
      {
         if (result.IsFailure)
            return result.Error;

         list.Add(result.Value);
      }

      return Result.Success<List<EnrichedShopSlotDetails>, Error>(list);
   }
}