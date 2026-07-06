using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;

namespace InventoryShop.Application.Services;

public sealed class EnrichedItemDetailsFactory(IPlayersRepository playersRepository, IMapper mapper)
{
   public async Task<EnrichedItemDetails> CreateAsync(ItemEntity item, CancellationToken ct)
   {
      PlayerEntity? owner = item.OwnerId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)item.OwnerId, ct);
      
      PlayerEntity? creator = item.CreatorId is null 
         ? null 
         : await playersRepository.GetPlayerById((Guid)item.CreatorId, ct);
      
      return new EnrichedItemDetails
      {
         Id = item.Id,
         Description =  item.Description,
         Type = item.Type,
         StatsModifiers = mapper.Map<StatsDetails>(item.StatsModifiers),
         IsEquipped = item.IsEquipped,
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