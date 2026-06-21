using InventoryShop.Domain.Exceptions;

namespace InventoryShop.Domain.ValueObjects;

public sealed record Wallet(uint GoldAmount)
{
   public Wallet Add(Wallet other)
   {
      uint finalAmount = GoldAmount;

      try
      {
         checked
         {
            finalAmount += other.GoldAmount;
         }
      }
      catch (Exception)
      {
         throw new InvalidWalletOperationException("Cannot add more gold than the wallet can hold");
      }
      
      return new Wallet(finalAmount);
   }

   public Wallet Subtract(Wallet other)
   {
      return other.GoldAmount > GoldAmount 
         ? throw new InvalidWalletOperationException("Cannot subtract more gold than the wallet contains") 
         : new Wallet(GoldAmount - other.GoldAmount);
   }

   public static Wallet CreateInitial() =>
      new(0);
}