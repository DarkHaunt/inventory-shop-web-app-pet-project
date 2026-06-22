using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedPlayerPolicyException(string message) : DomainException(message);