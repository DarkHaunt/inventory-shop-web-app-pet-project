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