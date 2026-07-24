using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class PlayMinigameValidator : AbstractValidator<PlayMinigameRequest>
{
   public PlayMinigameValidator()
   {
      RuleFor(x => x.PlayerId).NotEmpty().NotNull().WithMessage("PlayerId is required");
   }
}

public sealed class ExecutePurchaseValidator : AbstractValidator<ExecutePurchaseRequest>
{
   public ExecutePurchaseValidator()
   {
      RuleFor(x => x.BuyerId).NotEmpty().NotNull().WithMessage("BuyerId is required");
      RuleFor(x => x.SlotToExecute).NotEmpty().NotNull().WithMessage("SlotToExecute is required");
   }
}