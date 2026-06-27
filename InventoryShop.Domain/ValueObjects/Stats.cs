namespace InventoryShop.Domain.ValueObjects;

public sealed record Stats(int Agility, int Strength, int Intelligence)
{
   public Stats Add(Stats other) =>
      new(Agility + other.Agility, Strength + other.Strength, Intelligence + other.Intelligence);

   public Stats Multiply(double multiplier) =>
      new(
         Agility: (int)Math.Round(Agility * multiplier),
         Strength: (int)Math.Round(Strength * multiplier),
         Intelligence: (int)Math.Round(Intelligence * multiplier)
      );

   public static Stats Create(int agility, int strength, int intelligence) =>
      new(agility, strength, intelligence);

   public static Stats CreateInitial() =>
      Create(0, 0, 0);
}