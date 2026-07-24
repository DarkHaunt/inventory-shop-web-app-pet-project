using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Gameplay;
using InventoryShop.Application.UseCases.Items;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Application.UseCases.Players;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Services;
using InventoryShop.Infrastructure.Authentication;
using InventoryShop.Infrastructure.Persistence;
using InventoryShop.Infrastructure.Repositories;
using InventoryShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Web.Bindings;

public static class ApplicationBindings
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services)
   {
      services.AddScoped<EnrichedItemDetailsFactory>();
      services.AddScoped<EnrichedOrderDetailsFactory>();
      services.AddScoped<EnrichedPlayerDetailsFactory>();
      services.AddScoped<EnrichedSlotDetailsFactory>();
      
      services.AddScoped<GetPlayersUseCase>();
      services.AddScoped<CreatePlayerUseCase>();
      services.AddScoped<LoginPlayerUseCase>();
      services.AddScoped<DeletePlayerUseCase>();
      
      services.AddScoped<GetItemsUseCase>();
      services.AddScoped<EquipItemUseCase>();
      services.AddScoped<CreateItemUseCase>();
      services.AddScoped<DeleteItemUseCase>();
      
      services.AddScoped<GetShopOrdersUseCase>();
      services.AddScoped<CreateShopOrderUseCase>();
      services.AddScoped<DeleteShopOrderUseCase>();
      
      services.AddScoped<GetShopSlotsUseCase>();
      services.AddScoped<CreateShopSlotUseCase>();
      services.AddScoped<DeleteShopSlotUseCase>();
      services.AddScoped<ModifyShopSlotUseCase>();
      
      services.AddScoped<MinigamePlayUseCase>();
      services.AddScoped<ShopPurchaseUseCase>();
      
      return services;
   }

   public static IServiceCollection AddDomainServices(this IServiceCollection services)
   {
      services.AddSingleton<ItemsStatsCalculator>();
      services.AddSingleton<ItemsCreateService>();
      services.AddSingleton<MinigameRewardCalculator>();
      services.AddTransient<SimpleRandomPrimitiveProvider>();
      
      return services;
   }
   
   public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddDbContext<InventoryShopDbContext>(options =>
         options.UseNpgsql(configuration.GetConnectionString("Default")));
      
      services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
      services.AddHostedService<AdminSeedService>();

      services.AddSingleton<IGuidProvider, SequentialGuidProvider>();
      services.AddScoped<ISecurityTokenProvider, JwtSecurityTokenProvider>();
      services.AddScoped<IPasswordHasher, PasswordHasher>();
      services.AddScoped<ITransactionManager, TransactionManager>();
      
      services.AddScoped<IPlayersRepository, PlayersRepository>();
      services.AddScoped<IItemsRepository, ItemsRepository>();
      services.AddScoped<IShopSlotsRepository, ShopSlotsRepository>();
      services.AddScoped<IShopOrdersRepository, ShopOrdersRepository>();

      return services;
   }
}