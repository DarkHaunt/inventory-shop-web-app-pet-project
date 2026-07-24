using AutoMapper;
using InventoryShop.Application.Common;
using InventoryShop.Application.UseCases.Items;
using InventoryShop.Web.DTO;
using InventoryShop.Web.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class ItemsController(
   GetItemsUseCase getItemsUseCase,
   EquipItemUseCase equipItemUseCase,
   CreateItemUseCase createItemUseCase,
   DeleteItemUseCase deleteItemUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetItemById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetItemByIdAsync(id, ct);

      if (items.IsFailure)
         return NotFound(items.Error);

      var dto = mapper.Map<ItemDTO>(items.Value);
      return Ok(dto);
   }

   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllItems()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsAsync(ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllItemsOwnedBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsOwnedByPlayerAsync(ownerId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllItemsEquippedBy([FromQuery] Guid equipperId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsEquippedByPlayerAsync(equipperId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllItemsCreatedBy([FromQuery] Guid creatorId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsCreatedByPlayerAsync(creatorId, ct);
      
      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }
   
   [HttpGet]
   [AllowAnonymous]
   public async Task<IActionResult> GetAllItemsOnSaleBy([FromQuery] Guid sellerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var items = await getItemsUseCase.GetAllItemsOnSaleByPlayerAsync(sellerId, ct);

      var dto = new GetItemsResponse(items.Select(mapper.Map<ItemDTO>).ToList());
      return Ok(dto);
   }

   [HttpPatch]
   [Authorize(Policy = Policies.RequireUser)]
   public async Task<IActionResult> EquipItem([FromBody] EquipItemByPlayerRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var equipResult = await equipItemUseCase.ExecuteAsync(request.ItemToEquipId, request.EquipperId, request.IsEquipped, ct);

      if (equipResult.IsFailure)
         return BadRequest(equipResult.Error);

      return NoContent();
   }

   [HttpPost]
   [Authorize(Policy = Policies.RequireUser)]
   public async Task<IActionResult> CreateItem([FromBody] CreateItemByPlayerRequest request)
   {
      if (!ModelState.IsValid)
         return ValidationProblem();
      
      CancellationToken ct = HttpContext.RequestAborted;
      var creationResult = await createItemUseCase.ExecuteAsync(creatorId: request.CreatorId, ct);

      if (creationResult.IsFailure)
         return BadRequest(creationResult.Error);

      var dto = mapper.Map<ItemDTO>(creationResult.Value);
      return Created(uri: HttpContext.Request.GetDisplayUrl(), value: dto);
   }
   
   [HttpPost]
   [Authorize(Policy = Policies.RequireAdmin)]
   public async Task<IActionResult> CreateItemBySystem()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var creationResult = await createItemUseCase.ExecuteAsync(creatorId: null, ct);

      if (creationResult.IsFailure)
         return BadRequest(creationResult.Error);

      var dto = mapper.Map<ItemDTO>(creationResult.Value);
      return Created(uri: HttpContext.Request.GetDisplayUrl(), value: dto);
   }

   [HttpDelete]
   [Authorize(Policy = Policies.RequireUser)]
   public async Task<IActionResult> DeleteItem([FromBody] DeletePlayerItemRequest request)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteItemUseCase.ExecuteAsync(User.IsInRole(Roles.Admin), request.ItemId, request.OwnerId, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}