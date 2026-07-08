using FluentValidation;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class CreateShopSlotValidator : AbstractValidator<CreateShopSlotRequest>
{
   public CreateShopSlotValidator()
   {
      RuleFor(x => x.SellerId).NotEmpty().NotNull().WithMessage("SellerId is required");
      RuleFor(x => x.ItemToSellId).NotEmpty().NotNull().WithMessage("ItemToSellId is required");
   }
}