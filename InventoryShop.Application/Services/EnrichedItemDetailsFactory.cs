using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Domain.Entities.Game;

namespace InventoryShop.Application.Services;

public sealed class EnrichedItemDetailsFactory(IMapper mapper)
{
   public EnrichedItemDetails Create(ItemEntity entity)
   {
      return new EnrichedItemDetails
      {
         Id = entity.Id,
         Description =  entity.Description,
         Type = entity.Type,
         StatsModifiers = mapper.Map<StatsDetails>(entity.StatsModifiers),
         IsEquipped = entity.IsEquipped,
         OwnerName = entity.Owner?.Nickname,
         CreatorName = entity.Creator?.Nickname
      };
   }
   
   public List<EnrichedItemDetails> CreateList(IEnumerable<ItemEntity> entities) =>
      entities.Select(Create).ToList();
}