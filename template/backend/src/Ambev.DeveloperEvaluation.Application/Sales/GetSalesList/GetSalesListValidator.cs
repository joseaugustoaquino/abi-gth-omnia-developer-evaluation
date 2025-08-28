using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesList;

/// <summary>
/// Validator for GetSalesListCommand
/// </summary>
public class GetSalesListValidator : AbstractValidator<GetSalesListCommand>
{
    /// <summary>
    /// Initializes validation rules for GetSalesListCommand
    /// </summary>
    public GetSalesListValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithMessage("Size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Size cannot exceed 100");

        RuleFor(x => x.CustomerId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Customer ID must be a valid GUID when provided");

        RuleFor(x => x.BranchId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Branch ID must be a valid GUID when provided");
    }
}