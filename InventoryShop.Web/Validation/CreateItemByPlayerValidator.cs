using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class CreateItemByPlayerValidator : AbstractValidator<CreateItemByPlayerRequest>
{
   public CreateItemByPlayerValidator()
   {
      RuleFor(x => x.CreatorId).NotEmpty().NotNull().WithMessage("CreatorId is required");
   }
}