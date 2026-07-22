using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Specifications;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Gameplay;

public sealed class MinigamePlayUseCase(
   ITransactionManager transactionManager, 
   IPlayersRepository playersRepository, 
   IItemsRepository itemsRepository, 
   EnrichedPlayerDetailsFactory enrichedPlayerDetailsFactory, 
   ItemsStatsCalculator itemsStatsCalculator, 
   MinigameRewardCalculator minigameRewardCalculator, 
   ILogger<MinigamePlayUseCase> logger)
{
   public async Task<Result<EnrichedPlayerDetails, Error>> ExecuteAsync(Guid playerId, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;
      
      PlayerEntity? playerToPlay = await playersRepository.GetPlayerById(playerId, ct);
      
      if (playerToPlay == null)
      {
         logger.LogError("Can't find player with {ID}", playerId);
         return PlayerErrors.PlayerWithIdNotFoundError(playerId);
      }

      var itemsS = new ItemsOwnedByPlayerSpecification(playerToPlay.Id);
      var itemsOwnedByPlayer = await itemsRepository.GetItemsSpecifiedAsync(itemsS, ct);
      var statsOfEquippedItems = itemsOwnedByPlayer.Where(i => i.IsEquipped).Select(i => i.StatsModifiers);
      Stats statsOfPlayer = itemsStatsCalculator.Calculate(statsOfEquippedItems);
      
      (LevelProgress newPlayerLevel, Wallet reward) = minigameRewardCalculator.CalculateReward(playerToPlay.LevelProgress, statsOfPlayer);
      playerToPlay.Deposit(reward);
      
      await playersRepository.UpdatePlayerLevelAsync(playerToPlay.Id, level: newPlayerLevel, ct);
      await playersRepository.UpdatePlayerWalletAsync(playerToPlay.Id, wallet: playerToPlay.Wallet, ct);

      EnrichedPlayerDetails playerDto = await enrichedPlayerDetailsFactory.Create(playerToPlay, ct);
      var commit = await transactionManager.CommitTransactionAsync(ct);
      return commit.IsFailure
         ? Result.Failure<EnrichedPlayerDetails, Error>(commit.Error)
         : Result.Success<EnrichedPlayerDetails, Error>(playerDto);
   }
}