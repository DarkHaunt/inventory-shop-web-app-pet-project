using AutoMapper;
using InventoryShop.Application.Common;
using InventoryShop.Application.UseCases.Gameplay;
using InventoryShop.Web.Bindings;
using InventoryShop.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class GameplayController(
   MinigamePlayUseCase minigamePlayUseCase,
   ShopPurchaseUseCase shopPurchaseUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpPost]
   [Authorize(Policy = Policies.RequireUser)]
   public async Task<IActionResult> PlayMinigame()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await minigamePlayUseCase.ExecuteAsync(User.GetUserId(), ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var playerDTO = mapper.Map<PlayerDTO>(result.Value);
      return Ok(value: playerDTO);
   }
   
   [HttpPost]
   [Authorize(Policy = Policies.RequireUser)]
   public async Task<IActionResult> PurchaseSlotItem([FromQuery] Guid slotId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      DateTime now = DateTime.UtcNow;
      Guid buyerId = User.GetUserId();
      
      var result = await shopPurchaseUseCase.ExecuteAsync(buyerId, slotId, now, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var orderDTO = mapper.Map<ShopOrderDTO>(result.Value);
      return Ok(value: orderDTO);
   }
}