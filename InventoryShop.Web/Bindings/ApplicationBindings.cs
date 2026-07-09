using System.Text;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Gameplay;
using InventoryShop.Application.UseCases.Items;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Application.UseCases.Players;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Services;
using InventoryShop.Infrastructure.Auth;
using InventoryShop.Infrastructure.Persistence;
using InventoryShop.Infrastructure.Repositories;
using InventoryShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
      
      services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

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
   
   public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
   {
      JwtOptions jwtOptions = configuration
                                 .GetSection(JwtOptions.SectionName)
                                 .Get<JwtOptions>()
                              ?? throw new InvalidOperationException("Jwt configuration section is missing.");
      
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
            options.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateAudience = false,
               ValidateIssuer = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtOptions.SecretKey)) 
            };
         });

      services.AddAuthorization();

      return services;
   }
}