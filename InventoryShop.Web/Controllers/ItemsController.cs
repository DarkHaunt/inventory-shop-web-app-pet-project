using AutoMapper;
using InventoryShop.Application.UseCases.Items;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ItemsController(
   GetItemsUseCase getItemsUseCase,
   EquipItemUseCase equipItemUseCase,
   CreateItemUseCase createItemUseCase,
   DeleteItemUseCase deleteItemUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetItemById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetItemById(id, ct);

      if (items.IsFailure)
         return NotFound(items.Error);

      var dto = mapper.Map<ItemDTO>(items.Value);
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllItems()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsAsync(ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllItemsOwnedBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsOwnedByPlayerAsync(ownerId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllItemsEquippedBy([FromQuery] Guid equipperId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsEquippedByPlayerAsync(equipperId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllItemsCreatedBy([FromQuery] Guid creatorId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsCreatedByPlayerAsync(creatorId, ct);
      
      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }
   
   [HttpGet]
   public async Task<IActionResult> GetAllItemsOnSaleBy([FromQuery] Guid sellerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsOnSaleByPlayerAsync(sellerId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpPatch]
   public async Task<IActionResult> EquipItem([FromBody] EquipItemByPlayerRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var equipResult = await equipItemUseCase.ExecuteAsync(request.ItemToEquipId, request.EquipperId, request.IsEquipped, ct);

      if (equipResult.IsFailure)
         return BadRequest(equipResult.Error);

      return NoContent();
   }

   // TODO: Check if api caller is creator
   [HttpPost]
   public async Task<IActionResult> CreateItem([FromBody] CreateItemByPlayerRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var creationResult = await createItemUseCase.ExecuteAsync(creatorId: request.CreatorId, ct);

      if (creationResult.IsFailure)
         return BadRequest(creationResult.Error);

      var dto = mapper.Map<ItemDTO>(creationResult.Value);
      return Created(uri: HttpContext.Request.GetDisplayUrl(), value: dto);
   }
   
   // TODO: Admin only
   [HttpPost]
   public async Task<IActionResult> CreateItemBySystem()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var creationResult = await createItemUseCase.ExecuteAsync(creatorId: null, ct);

      if (creationResult.IsFailure)
         return BadRequest(creationResult.Error);

      var dto = mapper.Map<ItemDTO>(creationResult.Value);
      return Created(uri: HttpContext.Request.GetDisplayUrl(), value: dto);
   }

   // TODO: Check if api caller is item owner
   [HttpDelete]
   public async Task<IActionResult> DeleteItem([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteItemUseCase.ExecuteAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}