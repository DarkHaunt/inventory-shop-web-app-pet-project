using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases;

public sealed class GetPlayersUseCase(IPlayersRepository playersRepository, IItemsRepository itemsRepository, 
   ItemsStatsCalculator statsCalculator, EnrichedItemDetailsFactory itemDetailsFactory, IMapper mapper)
{
   public async Task<List<AggregatedPlayerDetails>> GetAllPlayersAsync(CancellationToken ct)
   {
      var players = await playersRepository.GetAllPlayersAsync(ct);

      var list = new List<AggregatedPlayerDetails>(players.Count);

      foreach (PlayerEntity p in players)
      {
         AggregatedPlayerDetails dto = await AggregatedPlayerDetails(p, ct);
         list.Add(dto);
      }

      return list;
   }
   
   public async Task<Result<AggregatedPlayerDetails, Error>> GetPlayerByIdAsync(Guid id, CancellationToken ct)
   {
      PlayerEntity? player = await playersRepository.GetPlayerById(id, ct);

      if(player == null)
         return Result.Failure<AggregatedPlayerDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(id));
      
      AggregatedPlayerDetails dto = await AggregatedPlayerDetails(player, ct);
      return Result.Success<AggregatedPlayerDetails, Error>(dto);
   }

   private async Task<AggregatedPlayerDetails> AggregatedPlayerDetails(PlayerEntity player, CancellationToken ct)
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
         Items = itemDetailsFactory.CreateList(itemsOwnedByPlayer)
      };
   }
}