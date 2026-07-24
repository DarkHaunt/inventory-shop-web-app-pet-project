using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Items;

public sealed class DeleteItemUseCase(ITransactionManager transactionManager, IItemsRepository itemsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(DeleteItemCommand command, CancellationToken ct)
   {
      if (command.IsAdmin == false)
      {
         if (await itemsRepository.IsItemOwnedByPlayerAsync(command.ItemId, command.OwnerId, ct) == false)
            return ItemsErrors.PlayerDoesNotOwnItem(command.ItemId, command.OwnerId);
      }

      await itemsRepository.DeleteItemAsync(command.ItemId, ct);
      return await transactionManager.SaveChangesAsync(ct);
   }
}