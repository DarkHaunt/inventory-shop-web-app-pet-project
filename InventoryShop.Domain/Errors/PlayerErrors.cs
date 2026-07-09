using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class PlayerErrors
{
   public static Error PlayerWithIdNotFoundError(Guid playerId) => 
      new(ErrorCode.DomainError, $"Player with id {playerId} not found");
   
   public static Error PlayerWithNicknameNotFoundError(string playerNickname) => 
      new(ErrorCode.DomainError, $"Player {playerNickname} not found");
   
   public static Error IncorrectPasswordError(string playerNickname) => 
      new(ErrorCode.DomainError, $"Password for player {playerNickname} is incorrect");
   
   public static Error NicknameTaken(string nickname) =>
      new (ErrorCode.DomainError, $"Nickname {nickname} is already taken");
   
   public static Error PasswordTaken() =>
      new (ErrorCode.DomainError, $"Password you're trying to insert is already taken. Try with another");

   public static Error NotEnoughGoldError(Guid playerId) =>
      new (ErrorCode.DomainError, $"Player {playerId} has not enough gold");
}