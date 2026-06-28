using AutoMapper;
using InventoryShop.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ShopOrdersController(
   GetOrdersUseCase getOrdersUseCase,
   CreateOrderUseCase createOrderUseCase,
   DeleteOrderUseCase deleteOrderUseCase,
   IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetOrderById([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getOrdersUseCase.GetOrderById(id, ct);

      if (orders.IsFailure)
         return NotFound(orders.Error);

      var dto = mapper.Map<ShopOrderDTO>(orders.Value);
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrders()
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getOrdersUseCase.GetAllOrdersAsync(ct);

      var dto = new GetOrdersResponse(orders.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersMadeBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getOrdersUseCase.GetAllOrdersOwnedByPlayerAsync(ownerId, ct);

      var dto = new GetOrdersResponse(orders.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpGet]
   public async Task<IActionResult> GetAllOrdersPurchasedBy([FromQuery] Guid ownerId)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var orders = await getOrdersUseCase.GetAllOrdersOwnedByPlayerAsync(ownerId, ct);

      var dto = new GetOrdersResponse(orders.Select(mapper.Map<ShopOrderDTO>).ToList());
      return Ok(dto);
   }

   [HttpDelete]
   public async Task<IActionResult> DeleteOrders([FromQuery] Guid id)
   {
      CancellationToken ct = HttpContext.RequestAborted;
      var result = await deleteOrdersUseCase.ExecuteAsync(id, ct);

      if (result.IsFailure)
         return BadRequest(result.Error);

      return NoContent();
   }
}