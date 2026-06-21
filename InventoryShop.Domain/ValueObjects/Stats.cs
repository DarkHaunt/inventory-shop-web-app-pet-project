namespace InventoryShop.Domain.ValueObjects;

public sealed record Stats(int Agility, int Strength, int Intelligence)
{
   public Stats Add(Stats other) =>
      new(Agility + other.Agility, Strength + other.Strength, Intelligence + other.Intelligence);

   public static Stats CreateInitial() =>
      new (0, 0, 0);
}