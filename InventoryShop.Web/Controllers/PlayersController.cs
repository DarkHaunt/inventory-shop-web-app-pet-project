using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.UseCases.Players;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class PlayersController(
   GetPlayersUseCase getPlayersUseCase,
   CreatePlayerUseCase createPlayerUseCase,
   LoginPlayerUseCase loginPlayerUseCase,
   DeletePlayerUseCase deletePlayerUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllPlayers()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var players = await getPlayersUseCase.GetAllPlayersAsync(ct);

      var dto = new GetPlayersResponse(players.Select(mapper.Map<PlayerDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetPlayerById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;

      (_, var isFailure, EnrichedPlayerDetails? playerInfo, Error? error) = await getPlayersUseCase.GetPlayerByIdAsync(id, ct);

      if (isFailure)
         return NotFound(error);

      return Ok(mapper.Map<PlayerDTO>(playerInfo));
   }

   [HttpPost]
   [Authorize]
   public async Task<IActionResult> RegisterNewPlayer([FromBody] RegisterNewPlayerRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
      CancellationToken ct = HttpContext.RequestAborted;
      DateTime now = DateTime.UtcNow;
      var command = new RegisterPlayerCommand(request.Nickname, request.Password, now);
      var result = await createPlayerUseCase.ExecuteAsync(command, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return Created();
   }
   
   [HttpPost]
   [Authorize]
   public async Task<IActionResult> LoginPlayer([FromBody] LoginPlayerRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
      CancellationToken ct = HttpContext.RequestAborted;
      var command = new LoginPlayerCommand(request.Nickname, request.Password);
      var result = await loginPlayerUseCase.ExecuteAsync(command, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return Ok(result.Value);
   }

   // TODO: Admin only
   [HttpDelete]
   [Authorize]
   public async Task<IActionResult> DeletePlayer([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deletePlayerUseCase.ExecuteAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}