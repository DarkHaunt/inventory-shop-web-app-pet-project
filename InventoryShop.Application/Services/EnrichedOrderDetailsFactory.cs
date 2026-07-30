using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.Services;

public sealed class EnrichedOrderDetailsFactory(
   IPlayersRepository playersRepository, 
   IMapper mapper, 
   ILogger<EnrichedOrderDetailsFactory> logger)
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
      (
         order.Id,
         buyer.Nickname,
         seller?.Nickname,
         order.CompletedAtUtc,
         mapper.Map<OrderDataDetails>(order.OrderData)
      );
   }
   
   public async Task<List<EnrichedShopOrderDetails>> CreateManyAsync(IEnumerable<ShopOrderEntity> orders, CancellationToken ct)
   {
      var results = new List<EnrichedShopOrderDetails>();
      foreach (var order in orders)
         results.Add(await CreateAsync(order, ct));
      return results;
   }
}