using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedUserPolicyException(string message) : DomainException(message);