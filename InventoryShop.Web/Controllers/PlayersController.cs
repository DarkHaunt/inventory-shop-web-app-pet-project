using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.UseCases.Players;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class PlayersController(GetPlayersUseCase getPlayersUseCase, CreatePlayerUseCase createPlayerUseCase, 
   DeletePlayerUseCase deletePlayerUseCase, IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetAllPlayers()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var players = await getPlayersUseCase.GetAllPlayersAsync(ct);
      
      var dto = new GetAllPlayersResponse
      {
         Players = players.Select(mapper.Map<PlayerDTO>).ToList()
      };

      return Ok(dto);
   }
   
   [HttpGet]
   public async Task<IActionResult> GetPlayerById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      
      (_, var isFailure, AggregatedPlayerDetails? playerInfo, Error? error) = await getPlayersUseCase.GetPlayerByIdAsync(id, ct);
      
      if(isFailure)
         return NotFound(error);

      return Ok(mapper.Map<PlayerDTO>(playerInfo));
   }

   [HttpPost]
   public async Task<IActionResult> RegisterNewPlayer([FromBody] RegisterNewPlayerRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await createPlayerUseCase.ExecuteAsync(request.Nickname, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
         
      return Created();
   }
   
   [HttpDelete]
   public async Task<IActionResult> DeletePlayer([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deletePlayerUseCase.ExecuteAsync(id, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
         
      return NoContent();
   }
}