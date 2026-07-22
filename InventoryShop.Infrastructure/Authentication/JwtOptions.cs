namespace InventoryShop.Infrastructure.Authentication;

public sealed class JwtOptions
{
   public const string SectionName = "Jwt";

   public string SecretKey { get; init; } = string.Empty;
   public int ExpirationHours { get; init; } = 6;
}