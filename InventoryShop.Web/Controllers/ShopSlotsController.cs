using AutoMapper;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Http.Extensions;
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
   [HttpGet]
   public async Task<ActionResult<ShopSlotDTO>> GetSlotById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetSlotsByIdAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return Ok(mapper.Map<ShopSlotDTO>(result.Value));
   }

   [HttpGet]
   public async Task<ActionResult<GetShopSlotsResponse>> GetAllSlots()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetAllSlotsAsync(ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var dto = new GetShopSlotsResponse(result.Value.Select(mapper.Map<ShopSlotDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<ActionResult<GetShopSlotsResponse>> GetAllSlotsCreatedByPlayer([FromQuery] Guid playerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetAllSlotsCreatedByPlayerAsync(playerId, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var dto = new GetShopSlotsResponse(result.Value.Select(mapper.Map<ShopSlotDTO>).ToList());
      return Ok(dto);
   }
   
   [HttpGet]
   public async Task<ActionResult<GetShopSlotsResponse>> GetAllSlotsCreatedBySystem()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await getShopSlotsUseCase.GetAllSlotsCreatedByPlayerAsync(creatorId: null, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      var dto = new GetShopSlotsResponse(result.Value.Select(mapper.Map<ShopSlotDTO>).ToList());
      return Ok(dto);
   }

   [HttpPost]
   public async Task<ActionResult<ShopSlotDTO>> CreateShopSlot([FromBody] CreateShopSlotRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
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
      
      var slotDTO = mapper.Map<ShopSlotDTO>(result.Value);
      return Created(uri: HttpContext.Request.GetDisplayUrl(), value: slotDTO);
   }

   [HttpPatch]
   public async Task<ActionResult<ShopSlotDTO>> ModifyShopSlot([FromBody] ModifyShopSlotRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
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

   // TODO: Check for owner delete or admin
   [HttpDelete]
   public async Task<IActionResult> DeleteShopSlot([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteShopSlotUseCase.ExecuteAsync(id, ct);
      
      if(result.IsFailure)
         return BadRequest(result.Error);
      
      return NoContent();
   }
}