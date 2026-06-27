using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Items;

public sealed class GetItemsUseCase(IItemsRepository itemsRepository, EnrichedItemDetailsFactory itemDetailsFactory, ILogger logger)
{
   public async Task<Result<EnrichedItemDetails, Error>> GetItemById(Guid id, CancellationToken ct)
   {
      ItemEntity? item = await itemsRepository.GetItemByIdAsync(id, ct);

      if (item is null)
      {
         logger.LogError("Can't find item with {ID}", id);
         return Result.Failure<EnrichedItemDetails, Error>(ItemsErrors.ItemWithIdNotFoundError(id));
      }

      EnrichedItemDetails dto = itemDetailsFactory.Create(item);
      return Result.Success<EnrichedItemDetails, Error>(dto);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsAsync(CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsAsync(ct);
      return itemDetailsFactory.CreateList(items);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsOwnedByPlayerAsync(Guid ownerId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsOwnedByAsync(ownerId, ct);
      return itemDetailsFactory.CreateList(items);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsEquippedByPlayerAsync(Guid equipperId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsEquippedByAsync(equipperId, ct);
      return itemDetailsFactory.CreateList(items);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsCreatedByPlayerAsync(Guid creatorId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsCreatedByAsync(creatorId, ct);
      return itemDetailsFactory.CreateList(items);
   }
}