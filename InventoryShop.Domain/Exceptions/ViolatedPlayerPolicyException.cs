namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedPlayerPolicyException(string message) : DomainException(message);