using InventoryShop.Application.Common;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Infrastructure.Services;

public sealed class AdminSeedService(
   IServiceProvider serviceProvider) : IHostedService
{
   public async Task StartAsync(CancellationToken cancellationToken)
   {
      await using var scope = serviceProvider.CreateAsyncScope();
      
      var context = scope.ServiceProvider.GetRequiredService<InventoryShopDbContext>();
      var guidProvider = scope.ServiceProvider.GetRequiredService<IGuidProvider>();
      var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
      var logger = scope.ServiceProvider.GetRequiredService<ILogger<AdminSeedService>>();

      var adminExists = await context.Players
         .AnyAsync(p => p.Role == Roles.Admin, cancellationToken);

      if (adminExists)
      {
         logger.LogInformation("Admin already exists. Skipping seed.");
         return;
      }

      var admin = PlayerEntity.Create
      (
         id: guidProvider.CreateNew(),
         nickname: Admin.Name,
         role: Roles.Admin,
         passwordHashed: passwordHasher.Hash("admin"),
         createdAt: DateTime.UtcNow,
         wallet: Wallet.CreateInitial(),
         levelProgress: LevelProgress.CreateInitial()
      );

      await context.Players.AddAsync(admin, cancellationToken);
      await context.SaveChangesAsync(cancellationToken);
      
      logger.LogInformation("Admin seed completed.");
   }

   public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}