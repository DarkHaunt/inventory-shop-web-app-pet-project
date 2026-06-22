using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Persistence;

public class InventoryShopDbContext(DbContextOptions<InventoryShopDbContext> options) : DbContext(options)
{
   public DbSet<PlayerEntity> Players { get; set; }
   public DbSet<ItemEntity> Items { get; set; }
   public DbSet<ShopSlotEntity> ShopSlots { get; set; }
   public DbSet<ShopOrderEntity> ShopOrders { get; set; }
   
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.ApplyConfiguration(new PlayerEntityConfiguration());
      modelBuilder.ApplyConfiguration(new ItemEntityConfiguration());
      modelBuilder.ApplyConfiguration(new ShopSlotEntityConfiguration());
      modelBuilder.ApplyConfiguration(new ShopOrderEntityConfiguration());
      
      base.OnModelCreating(modelBuilder);
   }
}