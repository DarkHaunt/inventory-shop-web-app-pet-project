using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Items;

public sealed class CreateItemUseCase(
   IGuidProvider guidProvider,
   ITransactionManager transactionManager,
   IPlayersRepository playersRepository,
   IItemsRepository itemsRepository,
   EnrichedItemDetailsFactory enrichedItemDetailsFactory,
   ItemsCreateService itemsCreateService,
   ILogger logger)
{
   public async Task<Result<EnrichedItemDetails, Error>> ExecuteAsync(Guid? creatorId, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return Result.Failure<EnrichedItemDetails, Error>(beginTransactionResult.Error);

      var creationOfNewItem = creatorId is not null
         ? await CreateItemByPlayer((Guid)creatorId, ct)
         : await CreateItemBySystem(ct);

      if (creationOfNewItem.IsFailure)
         return Result.Failure<EnrichedItemDetails, Error>(creationOfNewItem.Error);

      var commit = await transactionManager.CommitTransactionAsync(ct);
      return commit.IsFailure
         ? Result.Failure<EnrichedItemDetails, Error>(commit.Error)
         : Result.Success<EnrichedItemDetails, Error>(creationOfNewItem.Value);
   }

   private async Task<Result<EnrichedItemDetails, Error>> CreateItemByPlayer(Guid creatorId, CancellationToken ct)
   {
      PlayerEntity? creator = await playersRepository.GetPlayerById(creatorId, ct);

      if (creator == null)
      {
         logger.LogError("Can't find creator with {ID}", creatorId);
         return Result.Failure<EnrichedItemDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(creatorId));
      }

      ItemEntity item = itemsCreateService.CreateNewByPlayer(creator, guidProvider.CreateNew());
      return await SaveItemToDatabase(item, ct);
   }

   private async Task<Result<EnrichedItemDetails, Error>> CreateItemBySystem(CancellationToken ct)
   {
      ItemEntity item = itemsCreateService.CreateNewBySystem(guidProvider.CreateNew());
      return await SaveItemToDatabase(item, ct);
   }

   private async Task<Result<EnrichedItemDetails, Error>> SaveItemToDatabase(ItemEntity item, CancellationToken ct)
   {
      try
      {
         await itemsRepository.AddItemAsync(item, ct);
         return Result.Success<EnrichedItemDetails, Error>(enrichedItemDetailsFactory.Create(item));
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Creation of item {ID} was cancelled", item.Id);
         return Result.Failure<EnrichedItemDetails, Error>(GenericErrors.OperationCanceledError());
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to save item {ID}", item.Id);
         return Result.Failure<EnrichedItemDetails, Error>(ItemsErrors.CreationFailed(item.Id));
      }
   }
}