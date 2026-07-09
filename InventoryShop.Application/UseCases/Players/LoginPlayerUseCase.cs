using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Players;

public sealed class LoginPlayerUseCase(
   IPlayersRepository playersRepository,
   ISecurityTokenProvider securityTokenProvider,
   IPasswordHasher passwordHasher)
{
   public async Task<Result<string, Error>> ExecuteAsync(LoginPlayerCommand command, CancellationToken ct)
   {
      PlayerEntity? player = await playersRepository.GetPlayerByNickname(command.Nickname, ct);
      
      if(player is null)
         return PlayerErrors.PlayerWithNicknameNotFoundError(command.Nickname);

      if (passwordHasher.Verify(command.Password, player.PasswordHashed) == false)
         return PlayerErrors.IncorrectPasswordError(command.Nickname);

      return securityTokenProvider.GenerateSecurityTokenFor(player);
   }
}