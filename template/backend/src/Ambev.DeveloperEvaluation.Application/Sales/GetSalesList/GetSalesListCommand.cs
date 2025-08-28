using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesList;

/// <summary>
/// Command for retrieving a paginated list of sales
/// </summary>
public class GetSalesListCommand : IRequest<GetSalesListResult>
{
    /// <summary>
    /// Gets or sets the page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Gets or sets the customer ID filter (optional)
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the branch ID filter (optional)
    /// </summary>
    public Guid? BranchId { get; set; }
}