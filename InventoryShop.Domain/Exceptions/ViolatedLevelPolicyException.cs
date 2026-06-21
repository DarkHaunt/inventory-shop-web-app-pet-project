namespace InventoryShop.Domain.Exceptions;

public class ViolatedLevelPolicyException(string message) : DomainException(message);