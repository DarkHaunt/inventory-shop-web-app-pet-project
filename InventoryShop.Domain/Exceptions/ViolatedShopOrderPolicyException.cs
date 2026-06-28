using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedShopOrderPolicyException(string message) : DomainException(message);