using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Slots;

public sealed class DeleteShopSlotUseCase(
   ITransactionManager transactionManager,
   IShopSlotsRepository shopSlotsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      await shopSlotsRepository.DeleteSlotAsync(id, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}