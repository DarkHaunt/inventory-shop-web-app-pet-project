using System.Linq.Expressions;
using InventoryShop.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryShop.Infrastructure.Configurations;

public static class ConfigExtension
{
   public static void OwnLevelProgress<T>(
      this EntityTypeBuilder<T> builder,
      Expression<Func<T, LevelProgress>> func) where T : class
   {
      builder.OwnsOne(func, lvl =>
      {
         lvl.Property(l => l.Level);
         lvl.Property(l => l.Experience);
         lvl.Ignore(l => l.ExperienceForNextLevel);
      });
   }
}