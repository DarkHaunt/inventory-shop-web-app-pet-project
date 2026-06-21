using InventoryShop.Application.Shared;

namespace InventoryShop.Infrastructure.Services;

public class SequentialGuidProvider : IGuidProvider
{
   public Guid CreateNew() =>
      Guid.CreateVersion7();
}