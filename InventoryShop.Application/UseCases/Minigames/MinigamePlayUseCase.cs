using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Minigames;

public sealed class MinigamePlayUseCase(ITransactionManager transactionManager, IPlayersRepository playersRepository, 
   IItemsRepository itemsRepository, AggregatedPlayerDetailsFactory aggregatedPlayerDetailsFactory, 
   ItemsStatsCalculator itemsStatsCalculator, MinigameRewardCalculator minigameRewardCalculator, ILogger logger)
{
   public async Task<Result<AggregatedPlayerDetails, Error>> ExecuteAsync(Guid playerId, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return Result.Failure<AggregatedPlayerDetails, Error>(beginTransactionResult.Error);
      
      PlayerEntity? playerToPlay = await playersRepository.GetPlayerById(playerId, ct);
      
      if (playerToPlay == null)
      {
         logger.LogError("Can't find player with {ID}", playerId);
         return Result.Failure<AggregatedPlayerDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(playerId));
      }
      
      var itemsOwnedByPlayer = await itemsRepository.GetAllItemsOwnedByAsync(playerToPlay.Id, ct);
      var statsOfEquippedItems = itemsOwnedByPlayer.Where(i => i.IsEquipped).Select(i => i.StatsModifiers);
      Stats statsOfPlayer = itemsStatsCalculator.Calculate(statsOfEquippedItems);
      (LevelProgress newPlayerLevel, Wallet playerNewWallet) = minigameRewardCalculator.CalculateReward(playerToPlay.LevelProgress, statsOfPlayer);
      
      await playersRepository.UpdatePlayerAsync(playerToPlay.Id, ct, level: newPlayerLevel, wallet: playerNewWallet);

      AggregatedPlayerDetails playerDto = await aggregatedPlayerDetailsFactory.Create(playerToPlay, ct);
      var commit = await transactionManager.CommitTransactionAsync(ct);
      return commit.IsFailure
         ? Result.Failure<AggregatedPlayerDetails, Error>(commit.Error)
         : Result.Success<AggregatedPlayerDetails, Error>(playerDto);
   }
}