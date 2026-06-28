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

      return Result.Success<EnrichedItemDetails, Error>(await itemDetailsFactory.CreateAsync(item, ct));
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsAsync(CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsAsync(ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsOwnedByPlayerAsync(Guid ownerId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsOwnedByAsync(ownerId, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsEquippedByPlayerAsync(Guid equipperId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsEquippedByAsync(equipperId, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsCreatedByPlayerAsync(Guid creatorId, CancellationToken ct)
   {
      var items = await itemsRepository.GetAllItemsCreatedByAsync(creatorId, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
}