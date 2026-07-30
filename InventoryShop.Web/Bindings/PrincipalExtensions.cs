using System.Security.Claims;
using InventoryShop.Application.Common;

namespace InventoryShop.Web.Bindings;

public static class PrincipalExtensions
{
   public static Guid GetUserId(this ClaimsPrincipal user)
   {
      Claim claim = user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("User ID claim missing");

      return Guid.Parse(claim.Value);
   }
   
   public static bool IsAdmin(this ClaimsPrincipal user) =>
      user.IsInRole(Roles.Admin);
}