namespace InventoryShop.Domain.Shared.Exceptions;

public sealed class DataIntegrityException(string message) : DomainException(message);
