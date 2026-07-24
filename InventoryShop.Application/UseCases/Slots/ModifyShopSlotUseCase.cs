using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Slots;


public sealed class ModifyShopSlotUseCase(
   ITransactionManager transactionManager, 
   IShopSlotsRepository shopSlotsRepository,
   EnrichedSlotDetailsFactory slotDetailsFactory,
   IMapper mapper,
   ILogger<ModifyShopSlotUseCase> logger) 
{
   public async Task<Result<EnrichedShopSlotDetails, Error>> ExecuteAsync(ModifyShopSlotCommand command, CancellationToken ct)
   {
      if (command.IsAdmin == false)
      {
         if(await shopSlotsRepository.IsSlotOwnedByPlayerAsync(command.SlotOwnerId, command.SlotId, ct) == false)
            return ShopSlotsErrors.SlotNotOwnedByPlayerError(command.SlotOwnerId, command.SlotId);
      }
      
      if (command.NewPrice == null && command.NewLevelRequired == null)
      {
         logger.LogError("No new price or level required provided");
         return ShopSlotsErrors.NoPriceOrLevelRequiredProvided();
      }

      ShopSlotEntity? slot = await shopSlotsRepository.GetSlotById(command.SlotId, ct);

      if (slot is null)
      {
         logger.LogError("Can't find shop slot with {ID}", command.SlotId);
         return ShopSlotsErrors.ShopSlotWithIdNotFoundError(command.SlotId);
      }
      
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;

      if(command.NewLevelRequired != null)
         await shopSlotsRepository.UpdateSlotRequiredLevelAsync(command.SlotId, mapper.Map<LevelProgress>(command.NewLevelRequired), ct);

      if(command.NewPrice != null)
         await shopSlotsRepository.UpdateSlotPriceAsync(command.SlotId, mapper.Map<Wallet>(command.NewPrice), ct);
      
      var commit = await transactionManager.CommitTransactionAsync(ct);
      return commit.IsFailure
         ? Result.Failure<EnrichedShopSlotDetails, Error>(commit.Error)
         : Result.Success<EnrichedShopSlotDetails, Error>(await slotDetailsFactory.CreateAsync(slot, ct));
   }
}