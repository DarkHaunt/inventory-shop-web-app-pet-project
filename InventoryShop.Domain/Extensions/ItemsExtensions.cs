using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Shared.Extensions;

namespace InventoryShop.Domain.Extensions;

public static class ItemsExtensions
{
   public static bool IsItemInvalid(this ItemType type) =>
      type.IsValueExist() == false || type == ItemType.Unknown;
}