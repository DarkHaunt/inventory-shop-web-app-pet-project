using CSharpFunctionalExtensions;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace InventoryShop.Infrastructure.Services;

public sealed class TransactionManager(ILogger<TransactionManager> logger, InventoryShopDbContext context) : ITransactionManager
{
   private IDbContextTransaction? _transaction;

   public async Task<UnitResult<Error>> BeginTransactionAsync(CancellationToken ct = default)
   {
      try
      {
         _transaction = await context.Database.BeginTransactionAsync(ct);
         return UnitResult.Success<Error>();
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Transaction begin was canceled");
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to begin transaction");
         return GenericErrors.TransactionError();
      }
   }

   public async Task<UnitResult<Error>> CommitTransactionAsync(CancellationToken ct = default)
   {
      if (_transaction is null)
         return GenericErrors.TransactionError("Trying to commit transaction, but the transaction has not been started");

      try
      {
         await context.SaveChangesAsync(ct);
         await _transaction.CommitAsync(ct);

         return UnitResult.Success<Error>();
      }
      catch (DbUpdateConcurrencyException  e)
      {
         logger.LogError(e, "Concurrency error occured on committing transaction");
         await RollbackTransaction(ct);
         return GenericErrors.TransactionError("Concurrency error occured on committing transaction");
      }
      catch (DbUpdateException ex) when (ex.InnerException is PostgresException pEx)
      {
         await RollbackTransaction(ct);
         return HandlePostgresException(pEx);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Transaction commit was canceled");
         await RollbackTransaction(ct);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to commit transaction");
         await RollbackTransaction(ct);
         return GenericErrors.TransactionError();
      }
      finally
      {
         await DisposeAndInvalidateTransaction();
      }
   }

   public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken ct = default)
   {
      try
      {
         await context.SaveChangesAsync(ct);
         return UnitResult.Success<Error>();
      }
      catch (DbUpdateConcurrencyException  e)
      {
         logger.LogError(e, "Concurrency error occured on save changes");
         await RollbackTransaction(ct);
         return GenericErrors.TransactionError("Concurrency error occured on save changes");
      }
      catch (DbUpdateException ex) when (ex.InnerException is PostgresException pEx)
      {
         await RollbackTransaction(ct);
         return HandlePostgresException(pEx);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Save changes was canceled");
         await RollbackTransaction(ct);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to save changes");
         await RollbackTransaction(ct);
         return GenericErrors.TransactionError();
      }
      finally
      {
         await DisposeAndInvalidateTransaction();
      }
   }

   public async Task<UnitResult<Error>> RollbackTransaction(CancellationToken ct = default)
   {
      if (_transaction is null)
      {
         logger.LogWarning("No transaction started, nothing to rollback");
         return UnitResult.Success<Error>();
      }

      try
      {
         await _transaction.RollbackAsync(ct);
         return UnitResult.Success<Error>();
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Rollback changes was canceled");
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to rollback changes");
         return GenericErrors.TransactionError();
      }
   }

   private UnitResult<Error> HandlePostgresException(PostgresException e)
   {
      if (string.Equals(e.SqlState, PostgresErrorCodes.UniqueViolation, StringComparison.OrdinalIgnoreCase))
      {
         logger.LogWarning("Unique constraint violated for field {ViolatedField}", e.ConstraintName);
         return GenericErrors.DBError($"Unique constraint violated for field {e.ConstraintName}");
      }

      if (string.Equals(e.SqlState, PostgresErrorCodes.ForeignKeyViolation, StringComparison.OrdinalIgnoreCase))
      {
         logger.LogWarning("Foreign key constraint violated for field {ViolatedField}", e.ConstraintName);
         return GenericErrors.DBError($"Foreign key constraint violated for field {e.ConstraintName}");
      }

      logger.LogWarning("Unknown database error: {ESqlState}", e.SqlState);
      return GenericErrors.DBError(e.SqlState);
   }

   public void Dispose()
   {
      _transaction?.Dispose();
      _transaction = null;
   }

   public async ValueTask DisposeAsync() =>
      await DisposeAndInvalidateTransaction();

   private async Task DisposeAndInvalidateTransaction()
   {
      if (_transaction != null)
      {
         await _transaction.DisposeAsync();
         _transaction = null;
      }
   }
}