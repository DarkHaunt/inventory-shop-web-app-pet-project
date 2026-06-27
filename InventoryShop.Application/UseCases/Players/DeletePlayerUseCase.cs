using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Players;

public sealed class DeletePlayerUseCase(ITransactionManager transactionManager, IPlayersRepository playersRepository, ILogger logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      try
      {
         await playersRepository.DeletePlayerAsync(id, ct);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Deletion of player {ID} was cancelled", id);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to delete player {ID}, reason: {Error}", id, e.Message);
         return PlayerErrors.DeletionFailed(id);
      }

      return await transactionManager.CommitTransactionAsync(ct);
   }
}