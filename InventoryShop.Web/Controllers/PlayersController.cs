using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.UseCases;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayersController(GetPlayersUseCase getPlayersUseCase, IMapper mapper) : ControllerBase
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
}