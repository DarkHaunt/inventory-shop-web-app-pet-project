using AutoMapper;
using FluentAssertions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public sealed class CreateShopSlotUseCaseTests
{
   private readonly Mock<IGuidProvider> _guidProviderMock = new();
   private readonly Mock<ITransactionManager> _transactionManagerMock = new();
   private readonly Mock<IPlayersRepository> _playersRepositoryMock = new();
   private readonly Mock<IShopSlotsRepository> _slotsRepositoryMock = new();
   private readonly Mock<IItemsRepository> _itemsRepositoryMock = new();
   private readonly Mock<IEnrichedSlotDetailsFactory> _factoryMock = new();
   private readonly Mock<ILogger<CreateShopSlotUseCase>> _loggerMock = new();
   private readonly Mock<IMapper> _mapperMock = new();
   private readonly CreateShopSlotUseCase _create;

   public CreateShopSlotUseCaseTests()
   {
      _create = new CreateShopSlotUseCase(
         _guidProviderMock.Object,
         _transactionManagerMock.Object,
         _playersRepositoryMock.Object,
         _slotsRepositoryMock.Object,
         _itemsRepositoryMock.Object,
         _mapperMock.Object,
         _loggerMock.Object,
         _factoryMock.Object
      );
   }

   [Fact]
   internal async Task ExecuteAsync_SystemSell_WhenItemNotFound_ReturnsError()
   {
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: null, itemId: itemId);

      _transactionManagerMock.SetupTransactionFull();

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync((ItemEntity?)null);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsFailure.Should().BeTrue();

      _slotsRepositoryMock.Verify(
         r => r.AddSlotAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task ExecuteAsync_SystemSell_WhenItemNotSystemOwned_ReturnsError()
   {
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: null, itemId: itemId);
      var item = ItemEntityFixture.Create(ownerId: Guid.NewGuid());

      _transactionManagerMock.SetupTransactionFull();

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsFailure.Should().BeTrue();

      _slotsRepositoryMock.Verify(
         r => r.AddSlotAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task ExecuteAsync_SystemSell_WithValidSystemItem_ReturnsSuccessAndAddsSlot()
   {
      var itemId = Guid.NewGuid();
      var newSlotId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: null, itemId: itemId);
      var item = ItemEntityFixture.Create(ownerId: null, itemId: itemId);
      var details = ShopSlotEntityFixture.CreateDetails(Guid.NewGuid());

      _transactionManagerMock.SetupTransactionFull();

      _guidProviderMock
         .Setup(g => g.CreateNew())
         .Returns(newSlotId);
      
      _mapperMock
         .Setup(m => m.Map<Wallet>(command.Price))
         .Returns(Wallet.Create(command.Price.GoldAmount));
      
      _mapperMock
         .Setup(m => m.Map<LevelProgress>(command.LevelRequired))
         .Returns(LevelProgress.Create(command.LevelRequired.Level, command.LevelRequired.Experience));

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);

      _factoryMock
         .Setup(f => f.CreateAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(details);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsSuccess.Should().BeTrue();

      _slotsRepositoryMock.Verify(
         r => r.AddSlotAsync(
            It.Is<ShopSlotEntity>(s => s.Id == newSlotId && s.SellItemId == itemId),
            It.IsAny<CancellationToken>()),
         Times.Once);

      _playersRepositoryMock.Verify(
         r => r.GetPlayerById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task ExecuteAsync_PlayerSell_WhenPlayerNotFound_ReturnsError()
   {
      var sellerId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: sellerId, itemId: Guid.NewGuid());

      _transactionManagerMock.SetupTransactionFull();

      _playersRepositoryMock
         .Setup(r => r.GetPlayerById(sellerId, It.IsAny<CancellationToken>()))
         .ReturnsAsync((PlayerEntity?)null);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsFailure.Should().BeTrue();

      _itemsRepositoryMock.Verify(
         r => r.GetItemByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task ExecuteAsync_PlayerSell_WhenItemNotOwnedByPlayer_ReturnsError()
   {
      var sellerId = Guid.NewGuid();
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: sellerId, itemId: itemId);
      var seller = PlayerEntityFixture.Create(id: sellerId);
      var item = ItemEntityFixture.Create(ownerId: Guid.NewGuid()); // чужой айтем

      _transactionManagerMock.SetupTransactionFull();

      _playersRepositoryMock
         .Setup(r => r.GetPlayerById(sellerId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(seller);

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsFailure.Should().BeTrue();

      _itemsRepositoryMock.Verify(
         r => r.UpdateItemSaleStatus(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task ExecuteAsync_PlayerSell_WhenItemIsEquipped_UnequipsAndUpdatesStatuses()
   {
      var sellerId = Guid.NewGuid();
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: sellerId, itemId: itemId);
      var seller = PlayerEntityFixture.Create(id: sellerId);
      var item = ItemEntityFixture.Create(ownerId: sellerId, isEquipped: true);
      var details = ShopSlotEntityFixture.CreateDetails(Guid.NewGuid());

      _transactionManagerMock.SetupTransactionFull();

      _playersRepositoryMock
         .Setup(r => r.GetPlayerById(sellerId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(seller);

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);

      _factoryMock
         .Setup(f => f.CreateAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(details);
      
      _mapperMock
         .Setup(m => m.Map<Wallet>(command.Price))
         .Returns(Wallet.Create(command.Price.GoldAmount));
      
      _mapperMock
         .Setup(m => m.Map<LevelProgress>(command.LevelRequired))
         .Returns(LevelProgress.Create(command.LevelRequired.Level, command.LevelRequired.Experience));

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsSuccess.Should().BeTrue();
      item.IsEquipped.Should().BeFalse();

      _itemsRepositoryMock.Verify(
         r => r.UpdateItemEquipStatus(itemId, false, It.IsAny<CancellationToken>()),
         Times.Once);

      _itemsRepositoryMock.Verify(
         r => r.UpdateItemSaleStatus(itemId, true, It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   internal async Task ExecuteAsync_PlayerSell_WhenItemNotEquipped_StillCallsUpdateEquipStatus()
   {
      var sellerId = Guid.NewGuid();
      var itemId = Guid.NewGuid();
      var command = CreateValidCommand(sellerId: sellerId, itemId: itemId);
      var seller = PlayerEntityFixture.Create(id: sellerId);
      var item = ItemEntityFixture.Create(ownerId: sellerId, isEquipped: false);
      var details = ShopSlotEntityFixture.CreateDetails(Guid.NewGuid());

      _transactionManagerMock.SetupTransactionFull();

      _playersRepositoryMock
         .Setup(r => r.GetPlayerById(sellerId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(seller);
      
      _mapperMock
         .Setup(m => m.Map<Wallet>(command.Price))
         .Returns(Wallet.Create(command.Price.GoldAmount));
      
      _mapperMock
         .Setup(m => m.Map<LevelProgress>(command.LevelRequired))
         .Returns(LevelProgress.Create(command.LevelRequired.Level, command.LevelRequired.Experience));

      _itemsRepositoryMock
         .Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(item);

      _factoryMock
         .Setup(f => f.CreateAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(details);

      var result = await _create.ExecuteAsync(command, CancellationToken.None);

      result.IsSuccess.Should().BeTrue();

      _itemsRepositoryMock.Verify(
         r => r.UpdateItemEquipStatus(itemId, false, It.IsAny<CancellationToken>()),
         Times.Once);
   }
   
   private CreateShopSlotCommand CreateValidCommand(Guid? sellerId, Guid itemId)
   {
      var walletDetails = new WalletDetails(100);
      var levelDetails = new LevelProgressDetails(0, 0, 10);
      return new CreateShopSlotCommand(false, sellerId, itemId, walletDetails, levelDetails);
   }
}