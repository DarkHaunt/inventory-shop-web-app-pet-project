using CSharpFunctionalExtensions;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;
using Moq;

namespace InventoryShop.Tests.Common;

public static class TransactionManagerFixture
{
   public static void SetupTransactionFull(this Mock<ITransactionManager> transactionManagerMock)
   {
      transactionManagerMock
         .Setup(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()))
         .ReturnsAsync(UnitResult.Success<Error>);
      
      transactionManagerMock
         .Setup(tm => tm.CommitTransactionAsync(It.IsAny<CancellationToken>()))
         .ReturnsAsync(UnitResult.Success<Error>);
   }
}