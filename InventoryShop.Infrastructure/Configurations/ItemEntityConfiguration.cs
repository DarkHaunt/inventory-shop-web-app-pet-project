using InventoryShop.Domain.Entities.Game;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryShop.Infrastructure.Configurations;

public class ItemEntityConfiguration : IEntityTypeConfiguration<ItemEntity>
{
   public void Configure(EntityTypeBuilder<ItemEntity> builder)
   {
      builder.HasKey(i => i.Id);
      
      builder.Property(i => i.Type).IsRequired();
      builder.Property(i => i.Description).IsRequired();
      builder.ComplexProperty(i => i.StatsModifiers).IsRequired();
      builder.Property(i => i.IsEquipped).IsRequired();
      builder.Property(i => i.IsEquipped).IsRequired();

      builder.Property(i => i.OwnerId).IsRequired(false);
      builder.HasOne(i => i.Owner);
      
      builder.Property(i => i.CreatorId).IsRequired(false);
      builder.HasOne(i => i.Creator);
      
      builder.Ignore(i => i.IsSystemOwned);
      builder.Ignore(i => i.IsSystemCreated);
   }
}