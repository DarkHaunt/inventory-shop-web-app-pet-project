using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class EquipItemByPlayerValidator : AbstractValidator<EquipItemByPlayerRequest>
{
   public EquipItemByPlayerValidator()
   {
      RuleFor(x => x.ItemId).NotEmpty().NotNull().WithMessage("ItemToEquipId is required");
   }
}