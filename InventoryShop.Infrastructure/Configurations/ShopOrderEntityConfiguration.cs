using InventoryShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryShop.Infrastructure.Configurations;

public class ShopOrderEntityConfiguration : IEntityTypeConfiguration<ShopOrderEntity>
{
   public void Configure(EntityTypeBuilder<ShopOrderEntity> builder)
   {
      builder.HasKey(o => o.Id);
      
      builder.Property(o => o.BuyerId).IsRequired();
      builder.Property(o => o.SellerId);
      builder.Property(o => o.CompletedAtUtc).IsRequired();
      builder.OwnsOne(o => o.OrderData, od =>
      {
         od.OwnsOne(o => o.ItemSnapshot, snapshot =>
         {
            snapshot.Property(s => s.Id);
            snapshot.Property(s => s.Type);
            snapshot.Property(s => s.Description);
            snapshot.Property(s => s.CreatorId);
            snapshot.OwnsOne(s => s.StatsModifiers);
         });
         od.OwnsOne(o => o.Price);
         od.Property(o => o.RequiredLevel);
      });
   }
}