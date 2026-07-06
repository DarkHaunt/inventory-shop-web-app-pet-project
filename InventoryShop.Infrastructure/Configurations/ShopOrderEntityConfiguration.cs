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
      builder.ComplexProperty(o => o.OrderData, od =>
      {
         od.Property(d => d.RequiredLevel).IsRequired();
         od.ComplexProperty(d => d.ItemSnapshot);
         od.ComplexProperty(d => d.Price);
      });
   }
}