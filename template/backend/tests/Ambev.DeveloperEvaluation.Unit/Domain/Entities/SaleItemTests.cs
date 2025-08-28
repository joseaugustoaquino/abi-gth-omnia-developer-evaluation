using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact]
    public void SaleItem_WhenCreated_ShouldHaveCorrectInitialState()
    {
        // Arrange & Act
        var saleItem = new SaleItem();

        // Assert
        saleItem.Id.Should().NotBeEmpty();
        saleItem.ProductName.Should().BeEmpty();
        saleItem.Quantity.Should().Be(0);
        saleItem.UnitPrice.Should().Be(0);
        saleItem.DiscountPercentage.Should().Be(0);
        saleItem.TotalAmount.Should().Be(0);
        saleItem.IsCancelled.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 0)] // Less than 4 items
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    public void ApplyDiscountRules_WithLessThan4Items_ShouldNotApplyDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        saleItem.ApplyDiscountRules(0);

        // Assert
        saleItem.DiscountPercentage.Should().Be(expectedDiscount);
        saleItem.TotalAmount.Should().Be(quantity * 100m);
    }

    [Theory]
    [InlineData(4, 10)] // 4-9 items get 10% discount
    [InlineData(5, 10)]
    [InlineData(9, 10)]
    public void ApplyDiscountRules_With4To9Items_ShouldApply10PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        saleItem.ApplyDiscountRules(0);

        // Assert
        saleItem.DiscountPercentage.Should().Be(expectedDiscount);
        var expectedTotal = quantity * 100m * (1 - expectedDiscount / 100);
        saleItem.TotalAmount.Should().Be(expectedTotal);
    }

    [Theory]
    [InlineData(10, 20)] // 10-20 items get 20% discount
    [InlineData(15, 20)]
    [InlineData(20, 20)]
    public void ApplyDiscountRules_With10To20Items_ShouldApply20PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        saleItem.ApplyDiscountRules(0);

        // Assert
        saleItem.DiscountPercentage.Should().Be(expectedDiscount);
        var expectedTotal = quantity * 100m * (1 - expectedDiscount / 100);
        saleItem.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public void ApplyDiscountRules_WithMoreThan20Items_ShouldThrowException()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 21,
            UnitPrice = 100m
        };

        // Act & Assert
        var act = () => saleItem.ApplyDiscountRules(0);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot sell more than 20 identical items");
    }

    [Fact]
    public void CalculateTotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 5,
            UnitPrice = 100m,
            DiscountPercentage = 10m
        };

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        saleItem.TotalAmount.Should().Be(450m); // 5 * 100 * 0.9
    }

    [Fact]
    public void Cancel_WhenCalled_ShouldSetIsCancelledToTrue()
    {
        // Arrange
        var saleItem = new SaleItem();
        saleItem.IsCancelled.Should().BeFalse();

        // Act
        saleItem.Cancel();

        // Assert
        saleItem.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidSaleItem_ShouldNotThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 5,
            UnitPrice = 100m,
            DiscountPercentage = 10m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithEmptyProductName_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "",
            Quantity = 5,
            UnitPrice = 100m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithZeroQuantity_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 0,
            UnitPrice = 100m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithNegativeQuantity_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = -1,
            UnitPrice = 100m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithZeroUnitPrice_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 5,
            UnitPrice = 0m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithNegativeUnitPrice_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 5,
            UnitPrice = -10m
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WithInvalidDiscountPercentage_ShouldThrow(decimal discountPercentage)
    {
        // Arrange
        var saleItem = new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 5,
            UnitPrice = 100m,
            DiscountPercentage = discountPercentage
        };

        // Act & Assert
        var act = () => saleItem.Validate();
        act.Should().Throw<ArgumentException>();
    }
}