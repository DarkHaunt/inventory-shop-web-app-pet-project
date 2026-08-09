using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Tests.Common;

public static class PlayerEntityFixture
{
   public static PlayerEntity Create(Guid id) =>
      PlayerEntity.Create(
         id,
         "test-player",
         "user",
         "test-pass",
         DateTime.Now,
         Wallet.CreateInitial(),
         LevelProgress.CreateInitial()
      );
}