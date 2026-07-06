using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Commands;

public sealed record CreateShopOrderCommand(Guid BuyerId, Guid? SellerId, OrderData OrderData, DateTime DateOfCompletion);