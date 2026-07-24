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

public sealed class RegisterNewPlayerValidator : AbstractValidator<RegisterNewPlayerRequest>
{
   public RegisterNewPlayerValidator()
   {
      RuleFor(x => x.Nickname)
         .NotEmpty().WithMessage("Nickname is required")
         .MinimumLength(6).WithMessage("Nickname must be at least 6 characters.")
         .MaximumLength(24).WithMessage("Nickname must not exceed 24 characters.")
         .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Nickname may only contain letters, digits, and underscores.");

      RuleFor(x => x.Password)
         .NotEmpty().WithMessage("Password is required")
         .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
         .MaximumLength(16).WithMessage("Password must not exceed 16 characters.");
   }
}
