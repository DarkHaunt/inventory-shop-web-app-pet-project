using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities;

public sealed class PlayerEntity
{
   public Guid Id { get; private set; }
   public string Nickname { get; private set; }
   public string PasswordHashed { get; private set; }
   public DateTime CreatedAt { get; private set; }
   public Wallet Wallet { get; private set; }
   public LevelProgress LevelProgress { get; private set; }


   private PlayerEntity() { }

   public static PlayerEntity Create(Guid id, string nickname, string passwordHashed, DateTime createdAt, Wallet wallet, LevelProgress levelProgress)
   {
      if (string.IsNullOrWhiteSpace(nickname))
         throw new ViolatedPlayerPolicyException("Nickname is required for creating player");
      
      if (string.IsNullOrWhiteSpace(passwordHashed))
         throw new ViolatedPlayerPolicyException("Password is required for creating player");
      
      return new PlayerEntity
      {
         Id = id,
         Nickname = nickname,
         PasswordHashed = passwordHashed,
         CreatedAt = createdAt,
         Wallet = wallet,
         LevelProgress = levelProgress
      };
   }

   public void Deposit(Wallet amount) =>
      Wallet = Wallet.Deposit(amount);

   public void Withdraw(Wallet amount) =>
      Wallet = Wallet.Withdraw(amount);
}