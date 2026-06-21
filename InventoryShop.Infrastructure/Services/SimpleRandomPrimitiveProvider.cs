using InventoryShop.Application.Shared;

namespace InventoryShop.Infrastructure.Services;

public class SimpleRandomPrimitiveProvider(int? seed = null) : IRandomPrimitiveProvider
{
   private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

   public uint GetRandomUint() =>
      (uint)_random.NextInt64(minValue: 0, maxValue: uint.MaxValue);
}