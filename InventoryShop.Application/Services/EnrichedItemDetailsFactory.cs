using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;

namespace InventoryShop.Application.Services;

public interface IEnrichedItemDetailsFactory
{
   Task<EnrichedItemDetails> CreateAsync(ItemEntity item, CancellationToken ct);
   Task<List<EnrichedItemDetails>> CreateManyAsync(IEnumerable<ItemEntity> items, CancellationToken ct);
}

public sealed class EnrichedItemDetailsFactory(IPlayersRepository playersRepository, IMapper mapper) : IEnrichedItemDetailsFactory
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
      (
         item.Id,
         item.Type,
         item.Description,
         mapper.Map<StatsDetails>(item.StatsModifiers),
         item.IsEquipped,
         item.IsOnSale,
         owner?.Nickname,
         creator?.Nickname
      );
   }
   
   public async Task<List<EnrichedItemDetails>> CreateManyAsync(IEnumerable<ItemEntity> items, CancellationToken ct)
   {
      var results = new List<EnrichedItemDetails>();
      foreach (ItemEntity item in items)
         results.Add(await CreateAsync(item, ct));
      return results;
   }
}