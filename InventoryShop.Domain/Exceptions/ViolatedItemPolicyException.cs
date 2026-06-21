namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedItemPolicyException(string message) : DomainException(message);