using AutoMapper;
using InventoryShop.Application.UseCases.Gameplay;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class GameplayController(
   MinigamePlayUseCase minigamePlayUseCase,
   ShopPurchaseUseCase shopPurchaseUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpPost]
   [Authorize]
   public async Task<IActionResult> PlayMinigame([FromBody] PlayMinigameRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
      CancellationToken ct = HttpContext.RequestAborted;
      var playing = await minigamePlayUseCase.ExecuteAsync(request.PlayerId, ct);

      if (playing.IsFailure)
         return BadRequest(playing.Error);

      var playerDTO = mapper.Map<PlayerDTO>(playing.Value);
      return Ok(value: playerDTO);
   }
   
   [HttpPost]
   [Authorize]
   public async Task<IActionResult> PurchaseItem([FromBody] ExecutePurchaseRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
      CancellationToken ct = HttpContext.RequestAborted;
      DateTime now = DateTime.UtcNow;
      
      var purchasing = await shopPurchaseUseCase.ExecuteAsync(request.BuyerId, request.SlotToExecute, now, ct);

      if (purchasing.IsFailure)
         return BadRequest(purchasing.Error);

      var orderDTO = mapper.Map<ShopOrderDTO>(purchasing.Value);
      return Ok(value: orderDTO);
   }
}