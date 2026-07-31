using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

namespace InventoryShop.Domain.Tests;

public class ItemEntityTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly Guid ValidCreatorId = Guid.NewGuid();
    private static readonly Guid ValidOwnerId = Guid.NewGuid();

    private static Stats ValidStats() => 
        Stats.Create(10, 10, 10);

    private static ItemEntity CreateValidDefaultItem()
    {
         return ItemEntity.Create(
            ValidId,
            ItemType.Sword,
            "Test description",
            ValidStats(),
            ValidCreatorId,
            ValidOwnerId);
    }

    private ItemEntity CreateValidSystemOwnedItem(Guid? ownerId)
    {
        return ItemEntity.Create(
            ValidId,
            ItemType.Sword,
            "Test description",
            ValidStats(),
            ValidCreatorId,
            ownerId);
    }
    
    private ItemEntity CreateValidSystemCreatedItem(Guid? creatorId)
    {
        return ItemEntity.Create(
            ValidId,
            ItemType.Sword,
            "Test description",
            ValidStats(),
            creatorId,
            ValidOwnerId);
    }

    [Fact]
    public void Create_WithValidType_ReturnsItemWithExpectedState()
    {
        ItemEntity item = CreateValidDefaultItem();
        
        item.Id.Should().Be(ValidId);
        item.IsEquipped.Should().BeFalse();
        item.IsOnSale.Should().BeFalse();
        item.IsSystemOwned.Should().BeFalse();
        item.IsSystemCreated.Should().BeFalse();
    }

    [Fact]
    public void Create_WithUnknownType_ThrowsViolatedItemPolicyException()
    {
        var act = () => ItemEntity.Create(
            ValidId, ItemType.Unknown, "desc", ValidStats(), ValidCreatorId);

        act.Should().Throw<ViolatedItemPolicyException>();
    }

    [Fact]
    public void Create_WithOutOfRangeType_ThrowsViolatedItemPolicyException()
    {
        var act = () => ItemEntity.Create(
            ValidId, (ItemType)999, "desc", ValidStats(), ValidCreatorId);
        
        act.Should().Throw<ViolatedItemPolicyException>();
    }

    [Fact]
    public void Create_WithNullOwnerId_ProducesSystemOwnedItem()
    {
        ItemEntity item = CreateValidSystemOwnedItem(ownerId: null);

        item.IsSystemOwned.Should().BeTrue();
    }
    
    [Fact]
    public void Create_WithNullSystemId_ProducesSystemCreatedItem()
    {
        ItemEntity item = CreateValidSystemCreatedItem(creatorId: null);

        item.IsSystemCreated.Should().BeTrue();
    }

    [Fact]
    public void Equip_WhenSystemOwned_ThrowsViolatedItemPolicyException()
    {
        ItemEntity item = CreateValidSystemOwnedItem(ownerId: null);
        
        Action act = item.Equip;
        
        act.Should().Throw<ViolatedItemPolicyException>();
    }

    [Fact]
    public void Equip_WhenOwnedByPlayer_SetsIsEquippedTrue()
    {
        ItemEntity item = CreateValidDefaultItem();
        
        item.Equip();
        
        item.IsEquipped.Should().BeTrue();
    }

    [Fact]
    public void Unequip_WhenSystemOwned_ThrowsViolatedItemPolicyException()
    {
        ItemEntity item = CreateValidSystemOwnedItem(ownerId: null);
        
        Action act = item.Unequip;
        
        act.Should().Throw<ViolatedItemPolicyException>();
    }
    
    [Fact]
    public void TransferOwnershipTo_AlwaysResetsIsEquipped()
    {
        ItemEntity item = CreateValidDefaultItem();
        item.Equip();
        
        item.TransferOwnershipTo(Guid.NewGuid());

        item.IsEquipped.Should().BeFalse();
    }

    [Fact]
    public void TransferOwnershipTo_WithNull_MakesItemSystemOwned()
    {
        ItemEntity item = CreateValidDefaultItem();
        
        item.TransferOwnershipTo(null);

        item.IsSystemOwned.Should().BeTrue();
    }

    [Fact]
    public void IsOwnedBy_WithMatchingOwnerId_ReturnsTrue()
    {
        ItemEntity item = CreateValidDefaultItem();
        
        item.IsOwnedBy(ValidOwnerId).Should().BeTrue();
    }

    [Fact]
    public void IsOwnedBy_WithDifferentOwnerId_ReturnsFalse()
    {
        ItemEntity item = CreateValidDefaultItem();
        
        item.IsOwnedBy(Guid.NewGuid()).Should().BeFalse();
    }
}