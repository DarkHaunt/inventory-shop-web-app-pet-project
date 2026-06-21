namespace InventoryShop.Domain.Shared.Extensions;

public static class EnumExtensions
{
   public static bool IsValueExist<TEnum>(this TEnum enumValue) where TEnum : Enum =>
      Enum.IsDefined(typeof(TEnum), enumValue);
}