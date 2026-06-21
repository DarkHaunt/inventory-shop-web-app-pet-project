using InventoryShop.Domain.Exceptions;

namespace InventoryShop.Domain.ValueObjects;

public sealed record Level
{
   public const uint Max = 5;

   public uint Value { get; }

   public Level(uint value)
   {
      if (value > Max)
         throw new ViolatedLevelPolicyException($"Level must be lower than max {Max}");
      
      Value = value;
   }

   public static Level CreateInitial() => 
      new(0);
}