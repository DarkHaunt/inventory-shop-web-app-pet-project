using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Players;

public sealed class CreatePlayerUseCase(ITransactionManager transactionManager, IPlayersRepository playersRepository,
   IGuidProvider guidProvider, ILogger logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(string nickname, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      try
      { 
         if (await playersRepository.IsNicknameAlreadyExistsAsync(nickname, ct))
            return PlayerErrors.NicknameTaken(nickname);
         
         PlayerEntity player = CreatePlayerEntity(nickname);
         await playersRepository.AddPlayerAsync(player, ct);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Creation of player {Nickname} was cancelled", nickname);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to create player {Nickname}, reason: {Error}", nickname, e.Message);
         return PlayerErrors.CreationFailed(nickname);
      }

      return await transactionManager.CommitTransactionAsync(ct);
   }

   private PlayerEntity CreatePlayerEntity(string nickname) =>
      PlayerEntity.Create(guidProvider.CreateNew(), nickname, Wallet.CreateInitial(), LevelProgress.CreateInitial());
}