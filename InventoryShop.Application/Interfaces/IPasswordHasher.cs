using InventoryShop.Domain.Entities;

namespace InventoryShop.Application.Interfaces;

public interface IPasswordHasher
{
   string Hash(string passwordRaw);
   bool Verify(string passwordRaw, string passwordHashed);
}

public interface ISecurityTokenProvider
{
   string GenerateSecurityTokenFor(PlayerEntity player);
}