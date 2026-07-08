using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Items;

public sealed class GetItemsUseCase(
   IItemsRepository itemsRepository, 
   EnrichedItemDetailsFactory itemDetailsFactory, 
   ILogger logger)
{
   public async Task<Result<EnrichedItemDetails, Error>> GetItemByIdAsync(Guid id, CancellationToken ct)
   {
      ItemEntity? item = await itemsRepository.GetItemByIdAsync(id, ct);

      if (item is null)
      {
         logger.LogError("Can't find item {ID}", id);
         return ItemsErrors.ItemWithIdNotFoundError(id);
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
      var s = new ItemsOwnedByPlayerSpecification(ownerId);
      var items = await itemsRepository.GetItemsSpecifiedAsync(s, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsEquippedByPlayerAsync(Guid equipperId, CancellationToken ct)
   {
      var s = new ItemsEquippedByPlayerSpecification(equipperId);
      var items = await itemsRepository.GetItemsSpecifiedAsync(s, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsCreatedByPlayerAsync(Guid creatorId, CancellationToken ct)
   {
      var s = new ItemsCreatedByPlayerSpecification(creatorId);
      var items = await itemsRepository.GetItemsSpecifiedAsync(s, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
   
   public async Task<List<EnrichedItemDetails>> GetAllItemsOnSaleByPlayerAsync(Guid sellerId, CancellationToken ct)
   {
      var s = new ItemsOnSaleByPlayerSpecification(sellerId);
      var items = await itemsRepository.GetItemsSpecifiedAsync(s, ct);
      return await itemDetailsFactory.CreateManyAsync(items, ct);
   }
}