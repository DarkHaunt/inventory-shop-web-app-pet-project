using InventoryShop.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryShop.Infrastructure.Configurations;

public class ShopSlotEntityConfiguration : IEntityTypeConfiguration<ShopSlotEntity>
{
   public void Configure(EntityTypeBuilder<ShopSlotEntity> builder)
   {
      builder.HasKey(s => s.Id);
      
      builder.Property(s => s.SellItemId).IsRequired();
      builder.ComplexProperty(s => s.Price).IsRequired();
      builder.ComplexProperty(s => s.RequiredLevelProgress).IsRequired();
   }
}