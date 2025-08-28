using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale in the system with all business rules and validations
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale number
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer external ID (following DDD external identity pattern)
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name (denormalized for DDD external identity pattern)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch external ID where the sale was made
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch name (denormalized for DDD external identity pattern)
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the sale status
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the sale items
    /// </summary>
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Initializes a new instance of the Sale class
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        Status = SaleStatus.Active;
        SaleDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Applies business rules for discounts based on quantity
    /// </summary>
    public void ApplyDiscountRules()
    {
        // Business rules from README:
        // - 4+ items: 10% discount
        // - 10-20 items: 20% discount
        // - Cannot sell more than 20 items
        // - Less than 4 items: no discount

        if (Items == null || !Items.Any())
        {
            return;
        }

        var totalItems = Items.Sum(x => x.Quantity);

        // Apply discount rules based on total quantity
        var discountPercentage = 0m;
        if (totalItems >= 10 && totalItems <= 20)
        {
            discountPercentage = 0.20m;
        }
        else if (totalItems >= 4)
        {
            discountPercentage = 0.10m;
        }
            
        // Apply discount to each item
        foreach (var item in Items)
        {
            item.ApplyDiscountRules(discountPercentage);
        }

        CalculateTotalAmount();
    }

    /// <summary>
    /// Calculates the total amount based on sale items
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the sale
    /// </summary>
    public void Cancel()
    {
        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the sale entity
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var errors = new List<ValidationErrorDetail>();

        if (string.IsNullOrWhiteSpace(SaleNumber))
            errors.Add(new ValidationErrorDetail { Error = nameof(SaleNumber), Detail = "Sale number is required" });

        if (string.IsNullOrWhiteSpace(CustomerName))
            errors.Add(new ValidationErrorDetail { Error = nameof(CustomerName), Detail = "Customer name is required" });

        if (string.IsNullOrWhiteSpace(BranchName))
            errors.Add(new ValidationErrorDetail { Error = nameof(BranchName), Detail = "Branch name is required" });

        if (CustomerId == Guid.Empty)
            errors.Add(new ValidationErrorDetail { Error = nameof(CustomerId), Detail = "Customer ID is required" });

        if (BranchId == Guid.Empty)
            errors.Add(new ValidationErrorDetail { Error = nameof(BranchId), Detail = "Branch ID is required" });

        if (!Items.Any())
            errors.Add(new ValidationErrorDetail { Error = nameof(Items), Detail = "Sale must have at least one item" });

        // Validate each item
        foreach (var item in Items)
        {
            var itemValidation = item.Validate();
            if (!itemValidation.IsValid)
            {
                errors.AddRange(itemValidation.Errors);
            }
        }

        return new ValidationResultDetail
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}