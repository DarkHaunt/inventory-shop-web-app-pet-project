using FluentValidation;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Web.Requests;

namespace InventoryShop.Web.Validation;

public sealed class ModifyShopSlotValidator : AbstractValidator<ModifyShopSlotRequest>
{
   public ModifyShopSlotValidator()
   {
      RuleFor(x => x.SlotId).NotEmpty().NotNull().WithMessage("SlotId is required");
      RuleFor(x => x.NewPrice!.GoldAmount)
         .Must(p => p < 100_000)
         .WithMessage("Price must be less than 100000")
         .When(x => x.NewPrice != null);
      
      RuleFor(x => x.NewLevelRequired!.CurrentLevel)
         .Must(l => l <= LevelProgress.MaxLevel)
         .WithMessage($"Level must be less than or equal to {LevelProgress.MaxLevel}")
         .When(x => x.NewLevelRequired != null);;
   }
}

public sealed class CreateShopSlotValidator : AbstractValidator<CreateShopSlotRequest>
{
   public CreateShopSlotValidator()
   {
      RuleFor(x => x.ItemToSellId).NotEmpty().NotNull().WithMessage("ItemToSellId is required");
      RuleFor(x => x.Price).NotEmpty().NotNull().WithMessage("Price is required");
      RuleFor(x => x.LevelRequired).NotEmpty().NotNull().WithMessage("LevelRequired is required");
   }
}