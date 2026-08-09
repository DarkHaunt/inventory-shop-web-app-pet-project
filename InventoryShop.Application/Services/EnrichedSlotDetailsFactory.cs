using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public interface IEnrichedSlotDetailsFactory
{
   Task<EnrichedShopSlotDetails> CreateAsync(ShopSlotEntity slot, CancellationToken ct);
   Task<List<EnrichedShopSlotDetails>> CreateManyAsync(IEnumerable<ShopSlotEntity> slots, CancellationToken ct);
}

public sealed class EnrichedSlotDetailsFactory(
   IPlayersRepository playersRepository,
   IItemsRepository itemsRepository,
   EnrichedItemDetailsFactory enrichedItemDetailsFactory,
   IMapper mapper,
   ILogger<EnrichedSlotDetailsFactory> logger) : IEnrichedSlotDetailsFactory
{
   public async Task<EnrichedShopSlotDetails> CreateAsync(ShopSlotEntity slot, CancellationToken ct)
   {
      ItemEntity? itemToSell = await itemsRepository.GetItemByIdAsync(slot.SellItemId, ct);

      if (itemToSell is null)
      {
         logger.LogError("Item to sell with id {Id} not found", slot.SellerId);
         throw new DataIntegrityException($"Item {slot.SellItemId} referenced by slot {slot.Id} not found");
      }

      PlayerEntity? seller = slot.SellerId is null
         ? null
         : await playersRepository.GetPlayerById((Guid)slot.SellerId, ct);

      return new EnrichedShopSlotDetails
      (
         slot.Id,
         seller?.Nickname,
         await enrichedItemDetailsFactory.CreateAsync(itemToSell, ct),
         mapper.Map<WalletDetails>(slot.Price),
         slot.RequiredLevel.Level
      );
   }

   public async Task<List<EnrichedShopSlotDetails>> CreateManyAsync(IEnumerable<ShopSlotEntity> slots, CancellationToken ct)
   {
      var results = new List<EnrichedShopSlotDetails>();
      foreach (var slot in slots)
         results.Add(await CreateAsync(slot, ct));
      return results;
   }
}