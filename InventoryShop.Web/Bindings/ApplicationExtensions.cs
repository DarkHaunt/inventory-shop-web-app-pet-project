using System.Security.Claims;

namespace InventoryShop.Web.Bindings;

public static class ApplicationExtensions
{
   public static Guid GetUserId(this ClaimsPrincipal user)
   {
      Claim claim = user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("User ID claim missing");

      return Guid.Parse(claim.Value);
   }
}