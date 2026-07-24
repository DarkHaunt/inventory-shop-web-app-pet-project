using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Specifications;

namespace InventoryShop.Application.UseCases.Players;

public sealed class DeletePlayerUseCase(
   ITransactionManager transactionManager,
   IItemsRepository itemsRepository,
   IPlayersRepository playersRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid playerId, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      var ownedItems = new ItemsOwnedByPlayerSpecification(playerId);
      var playersItems = await itemsRepository.GetItemsSpecifiedAsync(ownedItems, ct);

      foreach (ItemEntity item in playersItems)
         await itemsRepository.DeleteItemAsync(item.Id, ct);
      
      await playersRepository.DeletePlayerAsync(playerId, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}