using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedOrderDetailsFactory(IPlayersRepository playersRepository, IMapper mapper, ILogger logger)
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> CreateAsync(ShopOrderEntity order, CancellationToken ct)
   {
      PlayerEntity? buyer = await playersRepository.GetPlayerById(order.BuyerId, ct);

      if (buyer is null)
      {
         logger.LogError("Buyer with id {Id} not found", order.BuyerId);
         return Result.Failure<EnrichedShopOrderDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(order.BuyerId));
      }
      
      PlayerEntity? seller = order.SellerId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)order.SellerId, ct);
      
      var dto = new EnrichedShopOrderDetails
      {
         Id = order.Id,
         CompletedAtUtc = order.CompletedAtUtc,
         
         BuyerName = buyer.Nickname,
         SellerName = seller?.Nickname,
         
         OrderData = mapper.Map<OrderDataDetails>(order.OrderData) 
      };
      
      return Result.Success<EnrichedShopOrderDetails, Error>(dto);
   }

   public async Task<Result<EnrichedShopOrderDetails, Error>[]> CreateManyAsync(IEnumerable<ShopOrderEntity> orders, CancellationToken ct) =>
      await Task.WhenAll(orders.Select(o => CreateAsync(o, ct)));
}