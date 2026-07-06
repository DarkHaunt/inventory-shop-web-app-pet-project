using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedSlotDetailsFactory(
   IPlayersRepository playersRepository,
   IItemsRepository itemsRepository,
   EnrichedItemDetailsFactory enrichedItemDetailsFactory,
   IMapper mapper,
   ILogger logger)
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
      {
         Id = slot.Id,
         SellerName = seller?.Nickname,

         SellItem = await enrichedItemDetailsFactory.CreateAsync(itemToSell, ct),
         Price = mapper.Map<WalletDetails>(slot.Price),
         RequiredLevel = slot.RequiredLevel.Level
      };
   }

   public async Task<List<EnrichedShopSlotDetails>> CreateManyAsync(IEnumerable<ShopSlotEntity> slots, CancellationToken ct)
   {
      var raw = await Task.WhenAll(slots.Select(o => CreateAsync(o, ct)));
      return raw.ToList();
   }
}