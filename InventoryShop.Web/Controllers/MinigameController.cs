using AutoMapper;
using InventoryShop.Application.UseCases.Minigames;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class MinigameController(
   MinigamePlayUseCase getItemsUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpPost]
   public async Task<IActionResult> PlayMinigame([FromBody] PlayMinigameRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var creationResult = await getItemsUseCase.ExecuteAsync(request.PlayerId, ct);

      if (creationResult.IsFailure)
         return BadRequest(creationResult.Error);

      var itemDTO = mapper.Map<PlayerDTO>(creationResult.Value);
      return Ok(value: itemDTO);
   }
}