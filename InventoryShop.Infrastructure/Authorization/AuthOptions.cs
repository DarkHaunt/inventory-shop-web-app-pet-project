namespace InventoryShop.Infrastructure.Authorization;

public sealed class AuthOptions
{
   public const string SectionName = "Auth";
   
   public string User { get; init; } = "user";
   public string Admin { get; init; } = "admin";
   
   public string RequireUser { get; init; } = "RequireUser";
   public string RequireAdmin { get; init; } = "RequireAdmin";
}