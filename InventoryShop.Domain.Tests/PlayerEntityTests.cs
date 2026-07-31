using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

namespace InventoryShop.Domain.Tests;

public sealed class PlayerEntityTests
{
   [Fact]
   internal void Create_WithZeroName_ShouldThrowException()
   {
      var act = () => PlayerEntity.Create(Guid.NewGuid(), "", "some-role", "some-password", DateTime.Now, Wallet.Create(100), LevelProgress.Create(1, 0));

      act.Should().Throw<ViolatedPlayerPolicyException>().WithMessage("Nickname is required for creating player");
   }
   
   [Fact]
   internal void Create_WithZeroRole_ShouldThrowException()
   {
      var act = () => PlayerEntity.Create(Guid.NewGuid(), "some-name", "", "some-password", DateTime.Now, Wallet.Create(100), LevelProgress.Create(1, 0));

      act.Should().Throw<ViolatedPlayerPolicyException>().WithMessage("Nickname is required for creating player");
   }

   [Fact]
   internal void Create_WithPassword_ShouldThrowException()
   {
      var act = () => PlayerEntity.Create(Guid.NewGuid(), "some-name", "some-role", "", DateTime.Now, Wallet.Create(100), LevelProgress.Create(1, 0));

      act.Should().Throw<ViolatedPlayerPolicyException>().WithMessage("Password is required for creating player");
   }

   [Fact]
   internal void Deposit_UpdatesPlayerWallet()
   {
      var player = PlayerEntity.Create(Guid.NewGuid(), "some-name", "some-role", "some-password", DateTime.Now, Wallet.Create(100), LevelProgress.Create(1, 0));
      
      player.Deposit(Wallet.Create(100));
      
      player.Wallet.GoldAmount.Should().Be(200);
   }
   
   [Fact]
   internal void Withdraw_UpdatesPlayerWallet()
   {
      var player = PlayerEntity.Create(Guid.NewGuid(), "some-name", "some-role", "some-password", DateTime.Now, Wallet.Create(200), LevelProgress.Create(1, 0));
      
      player.Withdraw(Wallet.Create(100));
      
      player.Wallet.GoldAmount.Should().Be(100);
   }
}