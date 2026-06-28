using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.DTO;

public record OrderDataDetails(ItemInOrderSnapshot ItemSnapshot, WalletDetails Price, uint RequiredLevel);