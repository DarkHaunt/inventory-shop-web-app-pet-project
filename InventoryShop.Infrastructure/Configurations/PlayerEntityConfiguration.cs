using InventoryShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryShop.Infrastructure.Configurations;

public class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
   public void Configure(EntityTypeBuilder<PlayerEntity> builder)
   {
      builder.HasKey(p => p.Id);
      builder.HasIndex(p => p.Nickname).IsUnique();
      
      builder.ComplexProperty(p => p.LevelProgress).IsRequired();
      builder.ComplexProperty(p => p.Wallet).IsRequired();
   }
}