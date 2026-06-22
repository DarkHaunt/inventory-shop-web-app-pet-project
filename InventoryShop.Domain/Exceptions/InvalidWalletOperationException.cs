using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Domain.Exceptions;

public class InvalidWalletOperationException(string message) : DomainException(message);