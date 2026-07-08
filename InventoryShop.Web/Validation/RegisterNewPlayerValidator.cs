using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class RegisterNewPlayerValidator : AbstractValidator<RegisterNewPlayerRequest>
{
   public RegisterNewPlayerValidator()
   {
      RuleFor(x => x.Nickname).NotEmpty().NotNull().WithMessage("Nickname is required");
   }
}