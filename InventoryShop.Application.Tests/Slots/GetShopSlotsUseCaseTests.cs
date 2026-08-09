using FluentAssertions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.UseCases.Slots;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Domain.Specifications;
using InventoryShop.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class GetShopSlotsUseCaseTests
{
   private readonly Mock<IShopSlotsRepository> _repositoryMock = new();
   private readonly Mock<IEnrichedSlotDetailsFactory> _factoryMock = new();
   private readonly Mock<ILogger<GetShopSlotsUseCase>> _loggerMock = new();
   private readonly GetShopSlotsUseCase _sut;

   public GetShopSlotsUseCaseTests()
   {
      _sut = new GetShopSlotsUseCase(
         _repositoryMock.Object,
         _factoryMock.Object,
         _loggerMock.Object);
   }

   [Fact]
   internal async Task GetSlotsByIdAsync_WhenSlotExists_ReturnsSuccessWithEnrichedDetails()
   {
      var slotId = Guid.NewGuid();
      var slot = ShopSlotEntityFixture.CreateSlot(slotId);
      var expectedDetails = ShopSlotEntityFixture.CreateDetails(slotId);

      _repositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(slot);

      _factoryMock
         .Setup(f => f.CreateAsync(slot, It.IsAny<CancellationToken>()))
         .ReturnsAsync(expectedDetails);

      var result = await _sut.GetSlotsByIdAsync(slotId, CancellationToken.None);

      result.IsSuccess.Should().BeTrue();
      expectedDetails.Should().Be(result.Value);
   }

   [Fact]
   internal async Task GetSlotsByIdAsync_WhenSlotDoesNotExist_ReturnsNotFoundError()
   {
      var slotId = Guid.NewGuid();

      _repositoryMock
         .Setup(r => r.GetSlotById(slotId, It.IsAny<CancellationToken>()))
         .ReturnsAsync((ShopSlotEntity?)null);

      var result = await _sut.GetSlotsByIdAsync(slotId, CancellationToken.None);
      result.IsFailure.Should().BeTrue();
      
      _factoryMock.Verify(
         f => f.CreateAsync(It.IsAny<ShopSlotEntity>(), It.IsAny<CancellationToken>()),
         Times.Never);
   }

   [Fact]
   internal async Task GetAllSlotsCreatedByPlayerAsync_PassesSpecificationWithGivenCreatorId()
   {
      var creatorId = Guid.NewGuid();
      SlotCreatedBySpecification? capturedSpec = null;

      _repositoryMock
         .Setup(r => r.GetSlotsSpecifiedAsync(It.IsAny<SlotCreatedBySpecification>(), It.IsAny<CancellationToken>()))
         .Callback<Specification<ShopSlotEntity>, CancellationToken>((spec, _) =>
            capturedSpec = (SlotCreatedBySpecification)spec)
         .ReturnsAsync([]);

      _factoryMock
         .Setup(f => f.CreateManyAsync(It.IsAny<IEnumerable<ShopSlotEntity>>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync([]);

      await _sut.GetAllSlotsCreatedByPlayerAsync(creatorId, CancellationToken.None);

      capturedSpec.Should().NotBeNull();
   }
}