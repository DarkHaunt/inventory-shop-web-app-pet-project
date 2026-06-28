using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Services;

namespace InventoryShop.Application.Services;

public sealed class AggregatedPlayerDetailsFactory(
   IItemsRepository itemsRepository,
   ItemsStatsCalculator statsCalculator,
   EnrichedItemDetailsFactory itemDetailsFactory,
   IMapper mapper)
{
   public async Task<AggregatedPlayerDetails> Create(PlayerEntity player, CancellationToken ct)
   {
      var itemsOwnedByPlayer = await itemsRepository.GetAllItemsOwnedByAsync(player.Id, ct);
      var statsOfEquippedItems = itemsOwnedByPlayer.Where(i => i.IsEquipped).Select(i => i.StatsModifiers);

      return new AggregatedPlayerDetails
      {
         Id = player.Id,
         Nickname = player.Nickname,
         Wallet = mapper.Map<WalletDetails>(player.Wallet),
         Stats = mapper.Map<StatsDetails>(statsCalculator.Calculate(statsOfEquippedItems)),
         LevelProgress = mapper.Map<LevelProgressDetails>(player.LevelProgress),
         Items = await itemDetailsFactory.CreateManyAsync(itemsOwnedByPlayer, ct)
      };
   }
}