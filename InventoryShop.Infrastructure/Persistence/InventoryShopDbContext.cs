using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Persistence;

public class InventoryShopDbContext(DbContextOptions<InventoryShopDbContext> options) : DbContext(options)
{
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
   }
}