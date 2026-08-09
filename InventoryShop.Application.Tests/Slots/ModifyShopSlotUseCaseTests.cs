using AutoMapper;
using FluentAssertions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public sealed class ModifyShopSlotUseCaseTests
{
   private readonly Mock<ITransactionManager> _transactionManagerMock = new();
   private readonly Mock<IShopSlotsRepository> _repositoryMock = new();
   private readonly Mock<IEnrichedSlotDetailsFactory> _factoryMock = new();
   private readonly Mock<ILogger<ModifyShopSlotUseCase>> _loggerMock = new();
   private readonly Mock<IMapper> _mapperMock = new();
   private readonly ModifyShopSlotUseCase _modify;

   public ModifyShopSlotUseCaseTests()
   {
      _modify = new ModifyShopSlotUseCase(
         _transactionManagerMock.Object,
         _repositoryMock.Object,
         _factoryMock.Object,
         _mapperMock.Object,
         _loggerMock.Object
      );
   }

   [Fact]
   internal async Task ModifySlot_WhichIOwned_ReturnsSuccess()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var command = CreateValidCommand(slotId, owner);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId, sellerId: owner);
      var details = ShopSlotEntityFixture.CreateDetails(id: slotId);
      
      _transactionManagerMock.SetupTransactionFull();

      _repositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);
      
      _repositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(true);
      
      _factoryMock
         .Setup(f => f.CreateAsync(slot, It.IsAny<CancellationToken>()))
         .ReturnsAsync(details);
      
      var mappedLevel = LevelProgress.Create(command.NewLevelRequired!.Level, command.NewLevelRequired.Experience);
      _mapperMock
         .Setup(m => m.Map<LevelProgress>(command.NewLevelRequired))
         .Returns(mappedLevel);
      
      var mappedWallet = Wallet.Create(command.NewPrice!.GoldAmount);
      _mapperMock
         .Setup(m => m.Map<Wallet>(command.NewPrice))
         .Returns(mappedWallet);

      var result = await _modify.ExecuteAsync(command, CancellationToken.None);
      
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().Be(details);
      
      _repositoryMock.Verify(
         r => r.UpdateSlotRequiredLevelAsync(slotId, mappedLevel, It.IsAny<CancellationToken>()),
         Times.Once);
      
      _repositoryMock.Verify(
         r => r.UpdateSlotPriceAsync(slotId, mappedWallet, It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   internal async Task ModifySlot_WhichIsNotOwned_ReturnsError()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var command = CreateValidCommand(slotId, owner);
      
      _repositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(false);

      var result = await _modify.ExecuteAsync(command, CancellationToken.None);
      
      result.IsFailure.Should().BeTrue();
   }

   [Fact]
   internal async Task ModifySlot_ByAdmin_EvenWhenIsNotOwned_ReturnsSuccess()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var command = CreateValidCommand(slotId, owner, isAdmin: true);
      var slot = ShopSlotEntityFixture.CreateSlot(id: slotId);
      var details = ShopSlotEntityFixture.CreateDetails(id: slotId);

      _repositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);

      _factoryMock
         .Setup(f => f.CreateAsync(slot, It.IsAny<CancellationToken>()))
         .ReturnsAsync(details);
      
      var mappedLevel = LevelProgress.Create(command.NewLevelRequired!.Level, command.NewLevelRequired.Experience);
      _mapperMock
         .Setup(m => m.Map<LevelProgress>(command.NewLevelRequired))
         .Returns(mappedLevel);
      
      var mappedWallet = Wallet.Create(command.NewPrice!.GoldAmount);
      _mapperMock
         .Setup(m => m.Map<Wallet>(command.NewPrice))
         .Returns(mappedWallet);

      _transactionManagerMock.SetupTransactionFull();

      var result = await _modify.ExecuteAsync(command, CancellationToken.None);

      result.IsSuccess.Should().BeTrue();
      
      _repositoryMock.Verify(
         r => r.IsSlotOwnedByPlayerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);
      
      _repositoryMock.Verify(
         r => r.UpdateSlotRequiredLevelAsync(slotId, mappedLevel, It.IsAny<CancellationToken>()),
         Times.Once);
      
      _repositoryMock.Verify(
         r => r.UpdateSlotPriceAsync(slotId, mappedWallet, It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   internal async Task ModifySlot_WhenLevelAndWalletAreEmpty_ReturnsError()
   {
      var slotId = Guid.NewGuid();
      var owner = Guid.NewGuid();
      var command = new ModifyShopSlotCommand(false, owner, slotId, null, null);
      
      _repositoryMock
         .Setup(r => r.IsSlotOwnedByPlayerAsync(owner, slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(true);

      var result = await _modify.ExecuteAsync(command, CancellationToken.None);
      
      result.IsFailure.Should().BeTrue();
      
      _repositoryMock.Verify(
         r => r.GetSlotById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   private ModifyShopSlotCommand CreateValidCommand(Guid slotId, Guid slotOwnerId, bool isAdmin = false)
   {
      var walletDetails = new WalletDetails(0);
      var levelDetails = new LevelProgressDetails(0, 0, 10);
      return new ModifyShopSlotCommand(isAdmin, slotOwnerId, slotId, walletDetails, levelDetails);
   }
}