using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Specifications;

namespace InventoryShop.Application.Services;

public sealed class EnrichedPlayerDetailsFactory(
   IItemsRepository itemsRepository,
   ItemsStatsCalculator statsCalculator,
   EnrichedItemDetailsFactory itemDetailsFactory,
   IMapper mapper)
{
   public async Task<EnrichedPlayerDetails> Create(PlayerEntity player, CancellationToken ct)
   {
      var itemsS = new ItemsOwnedByPlayerSpecification(player.Id);
      var itemsOwnedByPlayer = await itemsRepository.GetItemsSpecifiedAsync(itemsS, ct);
      var statsOfEquippedItems = itemsOwnedByPlayer.Where(i => i.IsEquipped).Select(i => i.StatsModifiers);

      return new EnrichedPlayerDetails
      (
         player.Id,
         player.Nickname,
         player.PasswordHashed,
         player.CreatedAt,
         mapper.Map<WalletDetails>(player.Wallet),
         mapper.Map<LevelProgressDetails>(player.LevelProgress),
         mapper.Map<StatsDetails>(statsCalculator.Calculate(statsOfEquippedItems)),
         await itemDetailsFactory.CreateManyAsync(itemsOwnedByPlayer, ct)
      );
   }
}