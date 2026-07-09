using InventoryShop.Application.Interfaces;

namespace InventoryShop.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
   public string Hash(string passwordRaw) =>
      BCrypt.Net.BCrypt.EnhancedHashPassword(passwordRaw);

   public bool Verify(string passwordRaw, string passwordHashed) =>
      BCrypt.Net.BCrypt.EnhancedVerify(passwordRaw, passwordHashed);
}