using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedOrderDetailsFactory(IPlayersRepository playersRepository, IMapper mapper, ILogger logger)
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> CreateAsync(ShopOrderEntity entity, CancellationToken ct)
   {
      PlayerEntity? buyer = await playersRepository.GetPlayerById(entity.BuyerId, ct);

      if (buyer is null)
      {
         logger.LogError("Buyer with id {Id} not found", entity.BuyerId);
         return Result.Failure<EnrichedShopOrderDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(entity.BuyerId));
      }
      
      PlayerEntity? seller = entity.SellerId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)entity.SellerId, ct);
      
      var dto = new EnrichedShopOrderDetails
      {
         Id = entity.Id,
         CompletedAtUtc = entity.CompletedAtUtc,
         
         BuyerName = buyer.Nickname,
         SellerName = seller?.Nickname,
         
         OrderData = mapper.Map<OrderDataDetails>(entity.OrderData) 
      };
      
      return Result.Success<EnrichedShopOrderDetails, Error>(dto);
   }

   public async Task<Result<EnrichedShopOrderDetails, Error>[]> CreateManyAsync(IEnumerable<ShopOrderEntity> orders, CancellationToken ct) =>
      await Task.WhenAll(orders.Select(o => CreateAsync(o, ct)));
}