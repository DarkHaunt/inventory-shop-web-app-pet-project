using AutoMapper;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ShopOrdersController(
   GetShopOrdersUseCase getShopOrdersUseCase,
   DeleteShopOrderUseCase deleteShopOrderUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetOrderById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetOrderByIdAsync(id, ct);

      if (orders.IsFailure)
         return NotFound(orders.Error);

      var dto = mapper.Map<ShopOrderDTO>(orders.Value);
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrders()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetAllOrdersAsync(ct);
      
      if (orders.IsFailure)
         return BadRequest(orders.Error);

      var dto = new GetShopOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersCompletedBy([FromQuery] Guid creatorId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetAllOrdersCompletedByPlayerAsync(creatorId, ct);

      if (orders.IsFailure)
         return BadRequest(orders.Error);

      var dto = new GetShopOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersCreatedBy([FromQuery] Guid creatorId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetAllOrdersCreatedByPlayerAsync(creatorId, ct);
      
      if (orders.IsFailure)
         return BadRequest(orders.Error);

      var dto = new GetShopOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   // TODO: Admin only
   [HttpDelete]
   public async Task<IActionResult> DeleteOrder([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteShopOrderUseCase.ExecuteAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}