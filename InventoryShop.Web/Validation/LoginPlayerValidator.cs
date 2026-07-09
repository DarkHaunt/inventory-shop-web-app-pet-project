using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class LoginPlayerValidator : AbstractValidator<LoginPlayerRequest>
{
   public LoginPlayerValidator()
   {
      RuleFor(x => x.Nickname).NotEmpty().NotNull().WithMessage("Nickname is required");
      RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required");
   }
}