using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Players;
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
      services.AddSingleton<AggregatedPlayerDetailsFactory>();
      
      services.AddScoped<GetPlayersUseCase>();
      services.AddScoped<CreatePlayerUseCase>();
      services.AddScoped<DeletePlayerUseCase>();
   }

   public static void AddDomainServices(this IServiceCollection services)
   {
      services.AddSingleton<ItemsStatsCalculator>();
      services.AddSingleton<MinigameRewardCalculator>();
      services.AddSingleton<LevelCalculator>();
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