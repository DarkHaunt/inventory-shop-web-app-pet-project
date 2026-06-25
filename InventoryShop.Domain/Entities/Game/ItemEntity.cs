using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.Extensions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities.Game;

public sealed class ItemEntity
{
   public Guid Id { get; private set; }
   public ItemType Type { get; private set; }
   public string Description { get; private set; }
   public Stats StatsModifiers { get; private set; }
   public bool IsEquipped { get; private set; }
   public Guid? OwnerId { get; private set; }
   public Guid? CreatorId { get; private init; }
   public PlayerEntity? Owner { get; private set; }
   public PlayerEntity? Creator { get; private set; }
   
   public bool IsSystemOwned => OwnerId is null;
   public bool IsSystemCreated => CreatorId is null;

   private ItemEntity() { }

   public static ItemEntity Create(Guid id, ItemType type, string description, Stats statsModifiers, PlayerEntity? creator, PlayerEntity? owner = null)
   {
      if (type.IsItemInvalid())
         throw new ViolatedItemPolicyException($"Item of type {type} is impossible to create");

      return new ItemEntity
      {
         Id = id,
         Type = type,
         Description = description,
         StatsModifiers = statsModifiers,
         CreatorId = creator?.Id,
         OwnerId = owner?.Id,
         Creator = creator,
         Owner = owner,
         IsEquipped = false
      };
   }

   public void TransferOwnershipTo(PlayerEntity? newOwner)
   {
      Owner = newOwner;
      OwnerId = newOwner?.Id;
      IsEquipped = false;
   }

   public void Equip()
   {
      if(OwnerId is null)
         throw new ViolatedItemPolicyException("Item is not owned by a player");
      
      IsEquipped = true;
   }

   public void Unequip()
   {
      if(OwnerId is null)
         throw new ViolatedItemPolicyException("Item is not owned by a player");
      
      IsEquipped = false;
   }
}