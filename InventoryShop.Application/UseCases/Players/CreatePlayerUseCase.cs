using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.Common;
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
   IPasswordHasher passwordHasher,
   IGuidProvider guidProvider)
{
   public async Task<UnitResult<Error>> ExecuteAsync(RegisterPlayerCommand command, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      if (await playersRepository.IsNicknameTakenAsync(command.Nickname, ct))
         return PlayerErrors.NicknameTaken(command.Nickname);

      string passwordHashed = passwordHasher.Hash(command.Password);
      
      if (await playersRepository.IsPasswordTakenAsync(passwordHashed, ct))
         return PlayerErrors.PasswordTaken();

      var player = PlayerEntity.Create
      (
         guidProvider.CreateNew(),
         command.Nickname,
         Roles.User,
         passwordHashed,
         command.CreatedAt,
         Wallet.CreateInitial(),
         LevelProgress.CreateInitial()
      );
      await playersRepository.AddPlayerAsync(player, ct);

      return await transactionManager.CommitTransactionAsync(ct);
   }
}