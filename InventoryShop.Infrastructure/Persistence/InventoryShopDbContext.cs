using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Entities.Shop;
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
      base.OnModelCreating(modelBuilder);
   }
}