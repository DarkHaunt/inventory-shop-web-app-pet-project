using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class EquipItemByPlayerValidator : AbstractValidator<EquipItemByPlayerRequest>
{
   public EquipItemByPlayerValidator()
   {
      RuleFor(x => x.ItemToEquipId).NotEmpty().NotNull().WithMessage("ItemToEquipId is required");
      RuleFor(x => x.EquipperId).NotEmpty().NotNull().WithMessage("EquipperId is required");
   }
}