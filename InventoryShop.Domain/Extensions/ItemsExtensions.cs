using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Shared.Extensions;

namespace InventoryShop.Domain.Extensions;

public static class ItemsExtensions
{
   public static bool IsItemValidType(this ItemType type) =>
      type.IsValueExist() && type != ItemType.Unknown;
}