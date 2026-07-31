using InventoryShop.Domain.Exceptions;

namespace InventoryShop.Domain.ValueObjects;

public sealed record Wallet(uint GoldAmount)
{
   public Wallet Deposit(Wallet other)
   {
      uint finalAmount = GoldAmount;

      try
      {
         checked
         {
            finalAmount += other.GoldAmount;
         }
      }
      catch (OverflowException)
      {
         throw new InvalidWalletOperationException("Cannot add more gold than the wallet can hold");
      }
      
      return Create(finalAmount);
   }

   public Wallet Withdraw(Wallet other)
   {
      return other.GoldAmount > GoldAmount 
         ? throw new InvalidWalletOperationException("Cannot subtract more gold than the wallet contains") 
         : Create(GoldAmount - other.GoldAmount);
   }

   public Wallet Multiply(double d)
   {
      uint finalAmount = GoldAmount;

      try
      {
         checked
         {
            finalAmount = (uint)Math.Round(finalAmount * d);
         }
      }
      catch (OverflowException)
      {
         throw new InvalidWalletOperationException("Cannot add more gold than the wallet can hold");
      }
      
      return Create(finalAmount);
   }
   
   public static Wallet Create(uint goldAmount) =>
      new(goldAmount);

   public static Wallet CreateInitial() =>
      Create(0);
}