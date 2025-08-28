using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a sale with business rules for discounts
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale ID this item belongs to
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product external ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name (denormalized)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item (after discount)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this item is cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the sale navigation property
    /// </summary>
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Calculates the total amount for this item
    /// </summary>
    public void CalculateTotalAmount()
    {
        var subtotal = Quantity * UnitPrice;
        var discountAmount = subtotal * (DiscountPercentage / 100);
        TotalAmount = subtotal - discountAmount;
    }

    /// <summary>
    /// Cancels this item
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }

    /// <summary>
    /// Validates the sale item
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var errors = new List<ValidationErrorDetail>();

        if (ProductId == Guid.Empty)
            errors.Add(new ValidationErrorDetail { Error = nameof(ProductId), Detail = "Product ID is required" });

        if (string.IsNullOrWhiteSpace(ProductName))
            errors.Add(new ValidationErrorDetail { Error = nameof(ProductName), Detail = "Product name is required" });

        if (Quantity <= 0)
            errors.Add(new ValidationErrorDetail { Error = nameof(Quantity), Detail = "Quantity must be greater than zero" });

        if (Quantity > 20)
            errors.Add(new ValidationErrorDetail { Error = nameof(Quantity), Detail = "Cannot sell more than 20 identical items" });

        if (UnitPrice <= 0)
            errors.Add(new ValidationErrorDetail { Error = nameof(UnitPrice), Detail = "Unit price must be greater than zero" });

        if (DiscountPercentage < 0 || DiscountPercentage > 100)
            errors.Add(new ValidationErrorDetail { Error = nameof(DiscountPercentage), Detail = "Discount percentage must be between 0 and 100" });

        // Validate discount rules
        if (Quantity < 4 && DiscountPercentage > 0)
            errors.Add(new ValidationErrorDetail { Error = nameof(DiscountPercentage), Detail = "Items with less than 4 quantity cannot have discount" });

        return new ValidationResultDetail
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}