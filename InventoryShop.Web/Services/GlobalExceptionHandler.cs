using InventoryShop.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace InventoryShop.Web.Services;

// Infrastructure / Web
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
   : IExceptionHandler
{
   public async ValueTask<bool> TryHandleAsync(
      HttpContext context, 
      Exception exception, 
      CancellationToken ct)
   {
      var (statusCode, message) = exception switch
      {
         ViolatedPlayerPolicyException e => (StatusCodes.Status400BadRequest, e.Message),
         ViolatedItemPolicyException e   => (StatusCodes.Status400BadRequest, e.Message),
         ViolatedLevelPolicyException e  => (StatusCodes.Status400BadRequest, e.Message),
         ViolatedShopSlotPolicyException e => (StatusCodes.Status400BadRequest, e.Message),
         InvalidWalletOperationException e => (StatusCodes.Status400BadRequest, e.Message),
         OperationCanceledException      => (StatusCodes.Status499ClientClosedRequest, "Request cancelled"),
         _                               => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
      };

      logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

      context.Response.StatusCode = statusCode;
      await context.Response.WriteAsJsonAsync(new { error = message }, ct);

      return true;
   }
}