using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Shared.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Services;

public sealed class ItemsCreateService(SimpleRandomPrimitiveProvider rnd)
{
   private static readonly Stats BaseItemStats = Stats.Create
   (
      agility: 1,
      strength: 1,
      intelligence: 1
   );

   public ItemEntity CreateNewByPlayer(PlayerEntity creator, Guid itemGuid) =>
      CreateNewItemByLevel(itemGuid, creatorId: creator.Id, levelOfCreator: creator.LevelProgress.Level);

   public ItemEntity CreateNewBySystem(Guid itemGuid) =>
      CreateNewItemByLevel(itemGuid, creatorId: null, levelOfCreator: null);

   private ItemEntity CreateNewItemByLevel(Guid itemGuid, Guid? creatorId, uint? levelOfCreator)
   {
      (ItemType itemType, var description) = GenerateItemDescription();

      return ItemEntity.Create
      (
         id: itemGuid,
         type: itemType,
         description: description,
         statsModifiers: CreateItemStats(levelOfCreator),
         creatorId: creatorId,
         ownerId: null
      );
   }

   private (ItemType, string) GenerateItemDescription()
   {
      ItemType type = rnd.GetRandomEnumValue(excludeValues: ItemType.Unknown);

      var desc = type switch
      {
         ItemType.Sword => "Shiny new sword",
         ItemType.Shield => "Wooden shield",
         ItemType.Bow => "Brand new bow",
         ItemType.MagicStaff => "Super magic staff",
         _ => throw new DomainException($"Can't find {type} item description")
      };
      
      return (type, desc);
   }

   private Stats CreateItemStats(uint? levelOfCreator)
   {
      Stats rolledItem = RandomStatsInDiapason();
      
      if(levelOfCreator is null)
         return RandomStatsInDiapason();

      double mult = 1f;

      for (int i = 0; i < levelOfCreator; i++)
         mult += rnd.GetRandomDouble(0.35d, 0.55d);
      
      return rolledItem.Multiply(mult);
   }

   private Stats RandomStatsInDiapason()
   {
      var rolledStrength = BaseItemStats.Strength + rnd.GetRandomInt(-2, 3);
      var rolledAgility = BaseItemStats.Agility + rnd.GetRandomInt(-2, 3);
      var rolledIntelligence = BaseItemStats.Intelligence + rnd.GetRandomInt(-2, 3);

      return Stats.Create(rolledAgility, rolledStrength, rolledIntelligence);
   }
}