using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Domain.Exceptions;

public sealed class ViolatedShopSlotPolicyException(string message) : DomainException(message);