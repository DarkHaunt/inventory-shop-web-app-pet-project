using FluentAssertions;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

public sealed class WalletTests
{
   [Fact]
   internal void CreateInitial_ShouldCreateEmptyWallet()
   {
      var wallet = Wallet.CreateInitial();

      wallet.GoldAmount.Should().Be(0);
   }
   
   [Fact]
   internal void Deposit_ShouldAddGoldAmounts()
   {
      var first = Wallet.Create(100);
      var second = Wallet.Create(40);

      Wallet result = first.Deposit(second);

      result.GoldAmount.Should().Be(140);
   }
   
   [Fact]
   internal void Deposit_ShouldThrow_WhenOverflowOccurs()
   {
      var wallet = Wallet.Create(uint.MaxValue);
      var other = Wallet.Create(1);

      Action act = () => wallet.Deposit(other);

      act.Should()
         .Throw<InvalidWalletOperationException>()
         .WithMessage("*wallet can hold*");
   }
   
   [Fact]
   internal void Withdraw_AllGold_ShouldReturnEmptyWallet()
   {
      var wallet = Wallet.Create(100);

      var result = wallet.Withdraw(Wallet.Create(100));

      result.GoldAmount.Should().Be(0);
   }
   
   [Fact]
   internal void Withdraw_ShouldThrow_WhenAmountIsGreaterThanBalance()
   {
      var wallet = Wallet.Create(100);

      Action act = () => wallet.Withdraw(Wallet.Create(101));

      act.Should()
         .Throw<InvalidWalletOperationException>()
         .WithMessage("*wallet contains*");
   }
   
   [Fact]
   internal void Multiply_ShouldMultiplyGold()
   {
      var wallet = Wallet.Create(100);

      var result = wallet.Multiply(2);

      result.GoldAmount.Should().Be(200);
   }
   
   [Fact]
   internal void Multiply_ShouldRoundResult()
   {
      var wallet = Wallet.Create(5);

      var result = wallet.Multiply(1.5);

      result.GoldAmount.Should().Be(8);
   }
   
   [Fact]
   internal void Multiply_ByZero_ShouldReturnEmptyWallet()
   {
      var wallet = Wallet.Create(100);

      var result = wallet.Multiply(0);

      result.GoldAmount.Should().Be(0);
   }
   
   [Fact]
   internal void Multiply_ShouldThrow_WhenOverflowOccurs()
   {
      var wallet = Wallet.Create(uint.MaxValue);

      Action act = () => wallet.Multiply(2);

      act.Should()
         .Throw<InvalidWalletOperationException>();
   }
}