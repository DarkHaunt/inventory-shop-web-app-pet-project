using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Items;

public sealed class DeleteItemUseCase(ITransactionManager transactionManager, IItemsRepository itemsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      await itemsRepository.DeleteItemAsync(id, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}