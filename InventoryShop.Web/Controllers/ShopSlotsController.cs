using AutoMapper;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ShopSlotsController(
   GetShopSlotsUseCase getShopSlotsUseCase,
   CreateShopSlotUseCase createShopSlotUseCase,
   DeleteShopSlotUseCase deleteShopSlotUseCase,
   ModifyShopSlotUseCase modifyShopSlotUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet("{id}")]
   public async Task<ActionResult<ShopSlotDTO>> GetSlotById(Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetSlotById(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return Ok(mapper.Map<ShopSlotDTO>(result.Value));
   }

   [HttpGet]
   public async Task<ActionResult<GetShopSlotsResponse>> GetAllSlotsAsync()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetAllSlotsAsync(ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var dto = new GetShopSlotsResponse(result.Value.Select(mapper.Map<ShopSlotDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet("player/slots/{playerId}")]
   public async Task<ActionResult<GetShopSlotsResponse>> GetAllSlotsCreatedByPlayerAsync([FromRoute] Guid? playerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetAllSlotsCreatedByPlayerAsync(playerId, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var dto = new GetShopSlotsResponse(result.Value.Select(mapper.Map<ShopSlotDTO>).ToList());
      return Ok(dto);
   }

   [HttpPost]
   public async Task<ActionResult<ShopSlotDTO>> CreateShopSlotAsync([FromBody] CreateShopSlotRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var command = new CreateShopSlotCommand(
         request.SellerId,
         request.ItemToSellId,
         mapper.Map<WalletDetails>(request.Price),
         mapper.Map<LevelProgressDetails>(request.LevelRequired)
      );
      
      var result = await createShopSlotUseCase.ExecuteAsync(command, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
      
      return Ok(mapper.Map<ShopSlotDTO>(result.Value));
   }

   [HttpPut("{id}")]
   public async Task<ActionResult<ShopSlotDTO>> ModifyShopSlotAsync([FromBody] ModifyShopSlotRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var command = new ModifyShopSlotCommand(
         request.Id,
         mapper.Map<WalletDetails>(request.NewPrice),
         mapper.Map<LevelProgressDetails>(request.NewLevelRequired)
      );
      
      var result = await modifyShopSlotUseCase.ExecuteAsync(command, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
      
      return Ok(mapper.Map<ShopSlotDTO>(result.Value));
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> DeleteShopSlotAsync(Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteShopSlotUseCase.ExecuteAsync(id, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
      
      return NoContent();
   }
}