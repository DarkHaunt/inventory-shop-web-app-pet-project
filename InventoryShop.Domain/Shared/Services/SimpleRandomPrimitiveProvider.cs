namespace InventoryShop.Domain.Services;

public interface ISimpleRandomPrimitiveProvider
{
   int GetRandomInt(int? min = null, int? max = null);
   uint GetRandomUint(uint? min = null, uint? max = null);
   long GetRandomLong(long? min = null, long? max = null);
   float GetRandomSingle(float? min = null, float? max = null);
   double GetRandomDouble(double? min = null, double? max = null);
   T GetRandomEnumValue<T>(params T[] excludeValues) where T : Enum;
}

public sealed class SimpleRandomPrimitiveProvider(int? seed = null) : ISimpleRandomPrimitiveProvider
{
   private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();
   
   public int GetRandomInt(int? min = null, int? max = null) =>
      _random.Next(minValue: min ?? int.MinValue, maxValue: max ?? int.MaxValue);

   public uint GetRandomUint(uint? min = null, uint? max = null) =>
      (uint)_random.NextInt64(minValue: min ?? uint.MinValue, maxValue: max ?? uint.MaxValue);
   
   public long GetRandomLong(long? min = null, long? max = null) =>
      _random.NextInt64(minValue: min ?? long.MinValue, maxValue: max ?? long.MaxValue);
   
   public float GetRandomSingle(float? min = null, float? max = null)
   {
      var t = _random.NextSingle();
      return float.Lerp(min ?? float.MinValue, max ?? float.MaxValue, t);
   }

   public double GetRandomDouble(double? min = null, double? max = null)
   {
      var t = _random.NextDouble();
      return double.Lerp(min ?? double.MinValue, max ?? double.MaxValue, t);
   }

   public T GetRandomEnumValue<T>(params T[] excludeValues) where T : Enum
   {
      var values = (T[])Enum.GetValues(typeof(T));
      values = values.Except(excludeValues).ToArray();
      return values[_random.Next(values.Length)];
   }
}