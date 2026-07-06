using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Items;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Application.UseCases.Players;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Services;
using InventoryShop.Infrastructure.Persistence;
using InventoryShop.Infrastructure.Repositories;
using InventoryShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Web.Bindings;

public static class ApplicationBindings
{
   public static void AddApplicationServices(this IServiceCollection services)
   {
      services.AddSingleton<EnrichedItemDetailsFactory>();
      services.AddSingleton<EnrichedOrderDetailsFactory>();
      services.AddSingleton<EnrichedPlayerDetailsFactory>();
      services.AddSingleton<EnrichedSlotDetailsFactory>();
      
      services.AddScoped<GetPlayersUseCase>();
      services.AddScoped<CreatePlayerUseCase>();
      services.AddScoped<DeletePlayerUseCase>();
      
      services.AddScoped<GetItemsUseCase>();
      services.AddScoped<CreateItemUseCase>();
      services.AddScoped<DeleteItemUseCase>();
      
      services.AddScoped<GetShopOrdersUseCase>();
      services.AddScoped<CreateShopOrderUseCase>();
      services.AddScoped<DeleteShopOrderUseCase>();
      
      services.AddScoped<GetShopSlotsUseCase>();
      services.AddScoped<CreateShopSlotUseCase>();
      services.AddScoped<DeleteShopSlotUseCase>();
      services.AddScoped<ModifyShopSlotUseCase>();
   }

   public static void AddDomainServices(this IServiceCollection services)
   {
      services.AddSingleton<ItemsStatsCalculator>();
      services.AddSingleton<MinigameRewardCalculator>();
      services.AddTransient<SimpleRandomPrimitiveProvider>();
   }
   
   public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddDbContext<InventoryShopDbContext>(options =>
         options.UseNpgsql(configuration.GetConnectionString("Default")));

      services.AddSingleton<IGuidProvider, SequentialGuidProvider>();
      services.AddScoped<ITransactionManager, TransactionManager>();
      
      services.AddScoped<IPlayersRepository, PlayersRepository>();
      services.AddScoped<IItemsRepository, ItemsRepository>();
      services.AddScoped<IShopSlotsRepository, ShopSlotsRepository>();
      services.AddScoped<IShopOrdersRepository, ShopOrdersRepository>();

      return services;
   }

}