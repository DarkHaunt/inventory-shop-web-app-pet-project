using AutoMapper;
using InventoryShop.Application.UseCases.Slots;
using Microsoft.AspNetCore.Mvc;

namespace InventoryShop.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ShopSlotsController(GetShopSlotsUseCase getShopSlotsUseCase, IMapper mapper)
{
   
}