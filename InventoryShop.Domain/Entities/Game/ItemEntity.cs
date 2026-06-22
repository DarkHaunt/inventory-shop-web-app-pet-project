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
   
   public bool IsSystemOwned => OwnerId is null;
   public bool IsSystemCreated => CreatorId is null;

   private ItemEntity() { }

   public static ItemEntity Create(Guid id, ItemType type, string description, Stats statsModifiers, Guid? creatorId, Guid? ownerId = null)
   {
      if (type.IsItemInvalid())
         throw new ViolatedItemPolicyException($"Item of type {type} is impossible to create");

      return new ItemEntity
      {
         Id = id,
         Type = type,
         Description = description,
         StatsModifiers = statsModifiers,
         CreatorId = creatorId,
         OwnerId = ownerId,
         IsEquipped = false
      };
   }

   public void TransferOwnershipTo(Guid newOwnerId)
   {
      OwnerId = newOwnerId;
      IsEquipped = false;
   }

   public void Equip() => 
      IsEquipped = true;
   
   public void Unequip() => 
      IsEquipped = false;
}