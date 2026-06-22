namespace InventoryShop.Domain.Services;

public sealed class SimpleRandomPrimitiveProvider(int? seed = null)
{
   private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

   public uint GetRandomUint() =>
      (uint)_random.NextInt64(minValue: 0, maxValue: uint.MaxValue);
   
   public double GetRandomDouble() =>
      _random.NextDouble();
}