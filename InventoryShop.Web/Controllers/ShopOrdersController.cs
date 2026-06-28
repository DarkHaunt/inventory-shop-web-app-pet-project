using AutoMapper;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ShopOrdersController(
   GetShopOrdersUseCase getShopOrdersUseCase,
   CreateShopOrderUseCase createShopOrderUseCase,
   DeleteShopOrderUseCase deleteShopOrderUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetOrderById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetOrderById(id, ct);

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

      var dto = new GetOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersCompletedBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetAllOrdersCompletedByPlayerAsync(ownerId, ct);

      if (orders.IsFailure)
         return BadRequest(orders.Error);

      var dto = new GetOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersCreatedBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getShopOrdersUseCase.GetAllOrdersCreatedByPlayerAsync(ownerId, ct);
      
      if (orders.IsFailure)
         return BadRequest(orders.Error);

      var dto = new GetOrdersResponse(orders.Value.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpDelete]
   public async Task<IActionResult> DeleteOrders([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteShopOrderUseCase.ExecuteAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}