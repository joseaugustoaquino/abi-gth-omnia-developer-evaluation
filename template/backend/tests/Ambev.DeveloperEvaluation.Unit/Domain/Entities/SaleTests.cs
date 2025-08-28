using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void Sale_WhenCreated_ShouldHaveCorrectInitialState()
    {
        // Arrange & Act
        var sale = new Sale();

        // Assert
        sale.Id.Should().NotBeEmpty();
        sale.SaleNumber.Should().BeEmpty();
        sale.SaleDate.Should().Be(default);
        sale.CustomerName.Should().BeEmpty();
        sale.TotalAmount.Should().Be(0);
        sale.BranchName.Should().BeEmpty();
        sale.Items.Should().NotBeNull().And.BeEmpty();
        sale.Status.Should().Be(SaleStatus.Active);
    }

    [Fact]
    public void ApplyDiscountRules_WithValidItems_ShouldApplyCorrectDiscounts()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = "Branch Test"
        };

        // Add 5 items (should get 10% discount)
        for (int i = 0; i < 5; i++)
        {
            sale.Items.Add(new SaleItem
            {
                ProductName = $"Product {i + 1}",
                Quantity = 1,
                UnitPrice = 100m
            });
        }

        // Act
        sale.ApplyDiscountRules();

        // Assert
        sale.Items.Should().AllSatisfy(item => item.DiscountPercentage.Should().Be(10));
        sale.TotalAmount.Should().Be(450m); // 5 * 100 * 0.9
    }

    [Fact]
    public void ApplyDiscountRules_With15Items_ShouldApply20PercentDiscount()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-002",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = "Branch Test"
        };

        // Add 15 items (should get 20% discount)
        for (int i = 0; i < 15; i++)
        {
            sale.Items.Add(new SaleItem
            {
                ProductName = $"Product {i + 1}",
                Quantity = 1,
                UnitPrice = 100m
            });
        }

        // Act
        sale.ApplyDiscountRules();

        // Assert
        sale.Items.Should().AllSatisfy(item => item.DiscountPercentage.Should().Be(20));
        sale.TotalAmount.Should().Be(1200m); // 15 * 100 * 0.8
    }

    [Fact]
    public void ApplyDiscountRules_WithLessThan4Items_ShouldNotApplyDiscount()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-003",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = "Branch Test"
        };

        // Add 3 items (should get no discount)
        for (int i = 0; i < 3; i++)
        {
            sale.Items.Add(new SaleItem
            {
                ProductName = $"Product {i + 1}",
                Quantity = 1,
                UnitPrice = 100m
            });
        }

        // Act
        sale.ApplyDiscountRules();

        // Assert
        sale.Items.Should().AllSatisfy(item => item.DiscountPercentage.Should().Be(0));
        sale.TotalAmount.Should().Be(300m); // 3 * 100
    }

    [Fact]
    public void Cancel_WhenCalled_ShouldSetIsCancelledToTrue()
    {
        // Arrange
        var sale = new Sale();
        sale.Status.Should().Be(SaleStatus.Active);

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public void Validate_WithValidSale_ShouldNotThrow()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = "Branch Test"
        };

        sale.Items.Add(new SaleItem
        {
            ProductName = "Product 1",
            Quantity = 1,
            UnitPrice = 100m
        });

        // Act & Assert
        var act = () => sale.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithEmptySaleNumber_ShouldThrow()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = "Branch Test"
        };

        // Act & Assert
        var act = () => sale.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithEmptyCustomer_ShouldThrow()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerName = "",
            BranchName = "Branch Test"
        };

        // Act & Assert
        var act = () => sale.Validate();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithEmptyBranch_ShouldThrow()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerName = "Customer Test",
            BranchName = ""
        };

        // Act & Assert
        var act = () => sale.Validate();
        act.Should().Throw<ArgumentException>();
    }
}