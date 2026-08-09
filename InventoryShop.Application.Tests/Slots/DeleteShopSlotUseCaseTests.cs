using FluentAssertions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Exceptions;
using InventoryShop.Tests.Common;
using Moq;
using Xunit;

public sealed class DeleteShopSlotUseCaseTests
{
   private readonly Mock<ITransactionManager> _transactionManagerMock = new();
   private readonly Mock<IShopSlotsRepository> _slotsRepositoryMock = new();
   private readonly Mock<IItemsRepository> _itemsRepositoryMock = new();
   private readonly DeleteShopSlotUseCase _del;

   public DeleteShopSlotUseCaseTests()
   {
      _del = new DeleteShopSlotUseCase(
         _transactionManagerMock.Object,
         _itemsRepositoryMock.Object,
         _slotsRepositoryMock.Object);
   }
   
   [Fact]
   internal async Task DeleteSlotAsync_WhenSlotIsOwnedByPlayer_ReturnsSuccess()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var item = ItemEntityFixture.Create(owner);
      var command = CreateValidCommand(slotId, owner);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId, sellerId: owner, sellItemId: item.Id);
      
      _transactionManagerMock.SetupTransactionFull();
      
      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(item.Id, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);
      
      _slotsRepositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);
      
      _slotsRepositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(true);
      
      var result = await _del.ExecuteAsync(command, CancellationToken.None);
      
      result.IsSuccess.Should().BeTrue();
      
      _slotsRepositoryMock.Verify(
         r => r.DeleteSlotAsync(slotId, It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   internal async Task DeleteSlotAsync_WhenUserIsAdmin_ReturnsSuccess()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var item = ItemEntityFixture.Create(owner);
      var command = CreateValidCommand(slotId, owner, isAdmin: true);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId, sellerId: owner, sellItemId: item.Id);
      
      _transactionManagerMock.SetupTransactionFull();
      
      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(item.Id, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);
      
      _slotsRepositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);
      
      _slotsRepositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(true);
      
      var result = await _del.ExecuteAsync(command, CancellationToken.None);
      result.IsSuccess.Should().BeTrue();
      
      _itemsRepositoryMock.Verify(
         r => r.IsItemOwnedByPlayerAsync(slotId, owner, It.IsAny<CancellationToken>()),
         Times.Never);
      
      _slotsRepositoryMock.Verify(
         r => r.DeleteSlotAsync(slotId, It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   internal async Task DeleteSlotAsync_WhenSlotIsNotOwnedByPlayer_ReturnsSlotNotOwnedByPlayerError()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var item = ItemEntityFixture.Create(owner);
      var command = CreateValidCommand(slotId, owner);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId, sellerId: owner, sellItemId: item.Id);
      
      _transactionManagerMock.SetupTransactionFull();
      
      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(item.Id, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);
      
      _slotsRepositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);
      
      _slotsRepositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(false);
      
      var result = await _del.ExecuteAsync(command, CancellationToken.None);
      
      result.IsFailure.Should().BeTrue();
      
      _slotsRepositoryMock.Verify(
         r => r.DeleteSlotAsync(slotId, It.IsAny<CancellationToken>()),
         Times.Never);
   }
   
   [Fact]
   internal async Task DeleteSlotAsync_WhenSlotDoesNotExist_ReturnsError()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var command = CreateValidCommand(slotId, owner, isAdmin: true);

      _slotsRepositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync((ShopSlotEntity?)null);

      var result = await _del.ExecuteAsync(command, CancellationToken.None);

      result.IsFailure.Should().BeTrue();

      _itemsRepositoryMock.Verify(
         r => r.GetItemByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);

      _transactionManagerMock.Verify(
         t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task DeleteSlotAsync_WhenItemInSlotDoesNotExist_ThrowsDataIntegrityException()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(slotId, owner, isAdmin: true);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId, sellerId: owner, sellItemId: itemId);

      _slotsRepositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync((ItemEntity?)null);

      Func<Task> act = () => _del.ExecuteAsync(command, CancellationToken.None);

      await act.Should().ThrowAsync<DataIntegrityException>();

      _transactionManagerMock.Verify(
         t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()),
         Times.Never);

      _slotsRepositoryMock.Verify(
         r => r.DeleteSlotAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   private DeleteShopSlotCommand CreateValidCommand(Guid slotId, Guid owner, bool isAdmin = false) =>
      new(isAdmin, owner, slotId);
}