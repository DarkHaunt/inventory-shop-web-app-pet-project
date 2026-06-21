using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities.Game;

public class PlayerEntity
{
   public Guid Id { get; private set; }
   public string Nickname { get; private set; }
   public Wallet Wallet { get; private set; }
   public Level Level { get; private set; }
   private List<ItemEntity> _inventory = new();
   public IReadOnlyList<ItemEntity> Inventory => _inventory;

   public Stats Stats => Inventory
      .Where(i => i.IsEquipped)
      .Aggregate(Stats.CreateInitial(), (acc, i) => acc.Add(i.StatsModifiers));

   private PlayerEntity() { }

   public static PlayerEntity Create(Guid id, string nickname)
   {
      if (string.IsNullOrWhiteSpace(nickname))
         throw new ViolatedPlayerPolicyException("Nickname is required for creating player");

      return new PlayerEntity
      {
         Id = id,
         Nickname = nickname,
         Wallet = Wallet.CreateInitial(),
         Level = Level.CreateInitial()
      };
   }
   
   public void Deposit(Wallet amount) => 
      Wallet = Wallet.Add(amount);
   
   public void Withdraw(Wallet amount) => 
      Wallet = Wallet.Subtract(amount);
}