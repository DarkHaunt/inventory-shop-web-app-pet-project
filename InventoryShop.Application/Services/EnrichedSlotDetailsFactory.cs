using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedSlotDetailsFactory(
   IPlayersRepository playersRepository,
   IItemsRepository itemsRepository,
   EnrichedItemDetailsFactory enrichedItemDetailsFactory,
   IMapper mapper,
   ILogger logger)
{
   public async Task<Result<EnrichedShopSlotDetails, Error>> CreateAsync(ShopSlotEntity slot, CancellationToken ct)
   {
      ItemEntity? itemToSell = await itemsRepository.GetItemByIdAsync(slot.SellItemId, ct);

      if (itemToSell is null)
      {
         logger.LogError("Item to sell with id {Id} not found", slot.SellerId);
         return Result.Failure<EnrichedShopSlotDetails, Error>(ItemsErrors.ItemWithIdNotFoundError(slot.SellItemId));
      }

      PlayerEntity? seller = slot.SellerId is null
         ? null
         : await playersRepository.GetPlayerById((Guid)slot.SellerId, ct);

      var dto = new EnrichedShopSlotDetails
      {
         Id = slot.Id,
         SellerName = seller?.Nickname,

         SellItem = await enrichedItemDetailsFactory.CreateAsync(itemToSell, ct),
         Price = mapper.Map<WalletDetails>(slot.Price),
         RequiredLevel = slot.RequiredLevel.Level
      };

      return Result.Success<EnrichedShopSlotDetails, Error>(dto);
   }

   public async Task<Result<EnrichedShopSlotDetails, Error>[]> CreateManyAsync(IEnumerable<ShopSlotEntity> slots, CancellationToken ct) =>
      await Task.WhenAll(slots.Select(o => CreateAsync(o, ct)));
}