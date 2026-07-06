using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Players;

public sealed class DeletePlayerUseCase(
   ITransactionManager transactionManager, 
   IPlayersRepository playersRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      await playersRepository.DeletePlayerAsync(id, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}