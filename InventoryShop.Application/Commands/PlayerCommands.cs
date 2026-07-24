namespace InventoryShop.Application.Commands;

public record RegisterPlayerCommand(
    string Nickname,
    string Password,
    DateTime CreatedAt
);

public record LoginPlayerCommand(
    string Nickname,
    string Password
);