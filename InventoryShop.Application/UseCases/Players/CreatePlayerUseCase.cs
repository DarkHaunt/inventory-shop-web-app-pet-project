using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.UseCases.Players;

public sealed class CreatePlayerUseCase(
   ITransactionManager transactionManager,
   IPlayersRepository playersRepository,
   IGuidProvider guidProvider)
{
   public async Task<UnitResult<Error>> ExecuteAsync(string nickname, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      if (await playersRepository.IsNicknameAlreadyExistsAsync(nickname, ct))
         return PlayerErrors.NicknameTaken(nickname);

      var player = PlayerEntity.Create
      (
         guidProvider.CreateNew(),
         nickname,
         Wallet.CreateInitial(),
         LevelProgress.CreateInitial()
      );
      await playersRepository.AddPlayerAsync(player, ct);

      return await transactionManager.CommitTransactionAsync(ct);
   }

   private PlayerEntity CreatePlayerEntity(string nickname) =>
      PlayerEntity.Create(guidProvider.CreateNew(), nickname, Wallet.CreateInitial(), LevelProgress.CreateInitial());
}