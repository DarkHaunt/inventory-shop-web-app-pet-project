using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.Shared.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Moq;
using Xunit;

namespace InventoryShop.Domain.Tests;

public sealed class ItemsCreateServiceTests
{
   private static readonly ItemType ItemType = ItemType.Sword;
   private static readonly Guid ValidItemId = Guid.NewGuid();
   private static readonly PlayerEntity ValidCreator = PlayerEntity.Create(
      Guid.NewGuid(),
      "test-player",
      "user",
      "test-pass",
      DateTime.Now,
      Wallet.CreateInitial(),
      LevelProgress.CreateInitial()
      );
   
   private ItemsCreateService CreateMockedValid()
   {
      var randomPrimitiveProvider = new Mock<ISimpleRandomPrimitiveProvider>();
      randomPrimitiveProvider.Setup(x => x.GetRandomInt()).Returns(1);
      randomPrimitiveProvider.Setup(x => x.GetRandomEnumValue<ItemType>()).Returns(ItemType);
      randomPrimitiveProvider.Setup(x => x.GetRandomEnumValue(ItemType.Unknown)).Returns(ItemType);
      randomPrimitiveProvider.Setup(x => x.GetRandomLong()).Returns(1L);
      randomPrimitiveProvider.Setup(x => x.GetRandomDouble()).Returns(1.0d);
      randomPrimitiveProvider.Setup(x => x.GetRandomUint()).Returns(1);
      randomPrimitiveProvider.Setup(x => x.GetRandomSingle()).Returns(1.0f);
      return new ItemsCreateService(randomPrimitiveProvider.Object);
   }
   
   private ItemsCreateService CreateMockedInvalid()
   {
      var randomPrimitiveProvider = new Mock<ISimpleRandomPrimitiveProvider>();
      randomPrimitiveProvider.Setup(x => x.GetRandomInt()).Returns(1);
      randomPrimitiveProvider.Setup(x => x.GetRandomEnumValue<ItemType>()).Returns(ItemType.Unknown);
      randomPrimitiveProvider.Setup(x => x.GetRandomEnumValue(ItemType.Unknown)).Returns(ItemType.Unknown);
      randomPrimitiveProvider.Setup(x => x.GetRandomLong()).Returns(1L);
      randomPrimitiveProvider.Setup(x => x.GetRandomDouble()).Returns(1.0d);
      randomPrimitiveProvider.Setup(x => x.GetRandomUint()).Returns(1);
      randomPrimitiveProvider.Setup(x => x.GetRandomSingle()).Returns(1.0f);
      return new ItemsCreateService(randomPrimitiveProvider.Object);
   }

   [Fact]
   internal void Create_WithPlayer_ReturnsItemWithExpectedState()
   {
      ItemsCreateService service = CreateMockedValid();
      
      ItemEntity item = service.CreateNewByPlayer(ValidCreator, ValidItemId);
        
      item.Id.Should().Be(ValidItemId);
      item.IsEquipped.Should().BeFalse();
      item.IsOnSale.Should().BeFalse();
      item.Type.Should().Be(ItemType);
      item.IsSystemOwned.Should().BeFalse();
      item.IsSystemCreated.Should().BeFalse();
   }
   
   [Fact]
   internal void Create_WithSystem_ReturnsItemWithExpectedState()
   {
      ItemsCreateService service = CreateMockedValid();
      
      ItemEntity item = service.CreateNewBySystem(ValidItemId);
        
      item.Id.Should().Be(ValidItemId);
      item.IsEquipped.Should().BeFalse();
      item.IsOnSale.Should().BeFalse();
      item.Type.Should().Be(ItemType);
      item.IsSystemOwned.Should().BeTrue();
      item.IsSystemCreated.Should().BeTrue();
   }
   
   [Fact]
   internal void Create_WithInvalidItemType_ReturnsDomainException()
   {
      ItemsCreateService service = CreateMockedInvalid();
      
      var act = () => service.CreateNewBySystem(ValidItemId);
      
      act.Should().Throw<DomainException>();
   }
 }