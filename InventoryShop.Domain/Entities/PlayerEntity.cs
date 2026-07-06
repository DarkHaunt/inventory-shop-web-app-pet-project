using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities;

public sealed class PlayerEntity
{
   public Guid Id { get; private set; }
   public string Nickname { get; private set; }
   public Wallet Wallet { get; private set; }
   public LevelProgress LevelProgress { get; private set; }


   private PlayerEntity() { }

   public static PlayerEntity Create(Guid id, string nickname, Wallet wallet, LevelProgress levelProgress)
   {
      if (string.IsNullOrWhiteSpace(nickname))
         throw new ViolatedPlayerPolicyException("Nickname is required for creating player");

      return new PlayerEntity
      {
         Id = id,
         Nickname = nickname,
         Wallet = wallet,
         LevelProgress = levelProgress
      };
   }
   
   public void Deposit(Wallet amount) => 
      Wallet = Wallet.Add(amount);
   
   public void Withdraw(Wallet amount) => 
      Wallet = Wallet.Subtract(amount);
}