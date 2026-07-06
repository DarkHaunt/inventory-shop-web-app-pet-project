using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedOrderDetailsFactory(IPlayersRepository playersRepository, IMapper mapper, ILogger logger)
{
   public async Task<EnrichedShopOrderDetails> CreateAsync(ShopOrderEntity order, CancellationToken ct)
   {
      PlayerEntity? buyer = await playersRepository.GetPlayerById(order.BuyerId, ct);

      if (buyer is null)
      {
         logger.LogError("Buyer with id {Id} not found", order.BuyerId);
         throw new DataIntegrityException($"Player {order.BuyerId} referenced by order {order.Id} not found");      
      }
      
      PlayerEntity? seller = order.SellerId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)order.SellerId, ct);

      return new EnrichedShopOrderDetails
      {
         Id = order.Id,
         CompletedAtUtc = order.CompletedAtUtc,
         
         BuyerName = buyer.Nickname,
         SellerName = seller?.Nickname,
         
         OrderData = mapper.Map<OrderDataDetails>(order.OrderData) 
      };
   }

   public async Task<List<EnrichedShopOrderDetails>> CreateManyAsync(IEnumerable<ShopOrderEntity> orders, CancellationToken ct)
   {
      var raw = await Task.WhenAll(orders.Select(o => CreateAsync(o, ct)));
      return raw.ToList();
   }
}