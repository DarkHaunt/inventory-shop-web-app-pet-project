using System.Text;
using InventoryShop.Application.Common;
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
using InventoryShop.Infrastructure.Caching;
using InventoryShop.Infrastructure.Persistence;
using InventoryShop.Infrastructure.Repositories;
using InventoryShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebApi.OpenApi;

namespace InventoryShop.Web.Bindings;

public static class BuilderExtensions
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

      services.AddScoped<IShopSlotsRepository, ShopSlotsRepository>();
      services.Decorate<IShopSlotsRepository, CachedShopSlotsRepository>();
      
      services.AddScoped<IShopOrdersRepository, ShopOrdersRepository>();
      services.Decorate<IShopOrdersRepository, CachedShopOrdersRepository>();
      
      services.AddScoped<IPlayersRepository, PlayersRepository>();
      services.Decorate<IPlayersRepository, CachedPlayerRepository>();
      
      services.AddScoped<IItemsRepository, ItemsRepository>();
      services.Decorate<IItemsRepository, CachedItemsRepository>();

      return services;
   }

   public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddStackExchangeRedisCache(options =>
      {
         options.Configuration = configuration.GetConnectionString("Redis");
         options.InstanceName = "InventoryShop:";
      });
      
      services.AddHybridCache(options =>
      {
         options.DefaultEntryOptions = new HybridCacheEntryOptions
         {
            Expiration = TimeSpan.FromMinutes(10), // Redis        
            LocalCacheExpiration = TimeSpan.FromMinutes(2) // Memory
         };
      });

      return services;
   }

   public static IServiceCollection AddAndSetupOpenApi(this IServiceCollection services)
   {
      services.AddOpenApi(options =>
      {
         options.AddDocumentTransformer((document, context, cancellationToken) =>
         {
            document.Info.Version = "1.0";
            document.Info.Title = "Inventory Shop API";
            document.Info.Description = "Web ASP.NET application implementation of inventory / shop system in some game";
            document.Info.Contact = new OpenApiContact
            {
               Name = "Yaroslav Kyzyk",
            };
            document.Info.License = new OpenApiLicense
            {
               Name = "MIT License",
               Url = new Uri("https://opensource.org/licenses/MIT")
            };
            return Task.CompletedTask;
         });
         options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
         options.AddOperationTransformer<AuthorizeOperationTransformer>();
      });

      return services;
   }

   public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
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

      return services;
   }

   public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddAuthorization(options =>
      {
         options.AddPolicy(Policies.RequireUser, policy =>
            policy
               .RequireAuthenticatedUser()
               .RequireRole(Roles.User, Roles.Admin));

         options.AddPolicy(Policies.RequireAdmin, policy =>
            policy
               .RequireAuthenticatedUser()
               .RequireRole(Roles.Admin));
      });

      return services;
   }
}