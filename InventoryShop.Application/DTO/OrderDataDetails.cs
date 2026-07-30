using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.DTO;

public record OrderDataDetails(
   ItemSnapshot ItemSnapshot,
   WalletDetails Price,
   uint RequiredLevel
);