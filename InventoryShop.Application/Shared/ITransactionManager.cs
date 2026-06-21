using CSharpFunctionalExtensions;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.Shared;

public interface ITransactionManager : IDisposable, IAsyncDisposable
{
   Task<UnitResult<Error>> BeginTransactionAsync(CancellationToken ct = default);
   Task<UnitResult<Error>> CommitTransactionAsync(CancellationToken ct = default);
   Task<UnitResult<Error>> SaveChangesAsync(CancellationToken ct = default);
   Task<UnitResult<Error>> RollbackTransaction(CancellationToken ct = default);
}