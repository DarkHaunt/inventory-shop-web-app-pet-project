using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace InventoryShop.Infrastructure.Authorization;

public class AdminRoleRequirement : IAuthorizationRequirement;

public sealed class AdminRoleHandler(IOptions<AuthOptions> options) : AuthorizationHandler<AdminRoleRequirement>
{
   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
   {
      if (context.User.IsInRole(options.Value.Admin))
         context.Succeed(requirement);

      return Task.CompletedTask;
   }
}