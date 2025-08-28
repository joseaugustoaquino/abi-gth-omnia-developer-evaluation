namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Response model for CancelSale operation
/// </summary>
public class CancelSaleResponse
{
    /// <summary>
    /// Indicates whether the cancellation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The unique identifier of the cancelled sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sale number
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;
}