using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Players;

public sealed class GetPlayersUseCase(IPlayersRepository playersRepository, AggregatedPlayerDetailsFactory aggregatedPlayerDetailsFactory, ILogger logger)
{
   public async Task<List<AggregatedPlayerDetails>> GetAllPlayersAsync(CancellationToken ct)
   {
      var players = await playersRepository.GetAllPlayersAsync(ct);

      var list = new List<AggregatedPlayerDetails>(players.Count);

      foreach (PlayerEntity p in players)
      {
         AggregatedPlayerDetails dto = await aggregatedPlayerDetailsFactory.Create(p, ct);
         list.Add(dto);
      }

      return list;
   }
   
   public async Task<Result<AggregatedPlayerDetails, Error>> GetPlayerByIdAsync(Guid id, CancellationToken ct)
   {
      PlayerEntity? player = await playersRepository.GetPlayerById(id, ct);

      if (player == null)
      {
         logger.LogError("Can't find player with {ID}", id);
         return Result.Failure<AggregatedPlayerDetails, Error>(PlayerErrors.PlayerWithIdNotFoundError(id));
      }
      
      AggregatedPlayerDetails dto = await aggregatedPlayerDetailsFactory.Create(player, ct);
      return Result.Success<AggregatedPlayerDetails, Error>(dto);
   }
}