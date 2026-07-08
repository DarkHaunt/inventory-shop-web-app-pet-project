using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Players;

public sealed class GetPlayersUseCase(
   IPlayersRepository playersRepository, 
   EnrichedPlayerDetailsFactory enrichedPlayerDetailsFactory, 
   ILogger logger)
{
   public async Task<Result<EnrichedPlayerDetails, Error>> GetPlayerByIdAsync(Guid id, CancellationToken ct)
   {
      PlayerEntity? player = await playersRepository.GetPlayerById(id, ct);

      if (player == null)
      {
         logger.LogError("Can't find player with {ID}", id);
         return PlayerErrors.PlayerWithIdNotFoundError(id);
      }
      
      EnrichedPlayerDetails dto = await enrichedPlayerDetailsFactory.Create(player, ct);
      return Result.Success<EnrichedPlayerDetails, Error>(dto);
   }

   public async Task<List<EnrichedPlayerDetails>> GetAllPlayersAsync(CancellationToken ct)
   {
      var players = await playersRepository.GetAllPlayersAsync(ct);

      var list = new List<EnrichedPlayerDetails>(players.Count);

      foreach (PlayerEntity p in players)
      {
         EnrichedPlayerDetails dto = await enrichedPlayerDetailsFactory.Create(p, ct);
         list.Add(dto);
      }

      return list;
   }
}