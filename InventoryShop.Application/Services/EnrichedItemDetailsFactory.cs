using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;

namespace InventoryShop.Application.Services;

public sealed class EnrichedItemDetailsFactory(IPlayersRepository playersRepository, IMapper mapper)
{
   public async Task<EnrichedItemDetails> CreateAsync(ItemEntity entity, CancellationToken ct)
   {
      PlayerEntity? owner = entity.OwnerId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)entity.OwnerId, ct);
      
      PlayerEntity? creator = entity.CreatorId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)entity.CreatorId, ct);
      
      return new EnrichedItemDetails
      {
         Id = entity.Id,
         Description =  entity.Description,
         Type = entity.Type,
         StatsModifiers = mapper.Map<StatsDetails>(entity.StatsModifiers),
         IsEquipped = entity.IsEquipped,
         OwnerName = owner?.Nickname,
         CreatorName = creator?.Nickname
      };
   }
   
   public async Task<List<EnrichedItemDetails>> CreateManyAsync(IEnumerable<ItemEntity> items, CancellationToken ct)
   {
      var raw = await Task.WhenAll(items.Select(o => CreateAsync(o, ct)));
      return raw.ToList();
   }
}