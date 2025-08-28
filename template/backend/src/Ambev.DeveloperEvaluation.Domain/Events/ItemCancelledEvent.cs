using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    /// <summary>
    /// Event raised when a sale item is cancelled
    /// </summary>
    public class ItemCancelledEvent
    {
        /// <summary>
        /// Gets the sale item that was cancelled
        /// </summary>
        public SaleItem SaleItem { get; }

        /// <summary>
        /// Gets the sale that contains the cancelled item
        /// </summary>
        public Sale Sale { get; }

        /// <summary>
        /// Gets the reason for item cancellation
        /// </summary>
        public string CancellationReason { get; }

        /// <summary>
        /// Initializes a new instance of ItemCancelledEvent
        /// </summary>
        /// <param name="saleItem">The sale item that was cancelled</param>
        /// <param name="sale">The sale that contains the cancelled item</param>
        /// <param name="cancellationReason">The reason for cancellation</param>
        public ItemCancelledEvent(SaleItem saleItem, Sale sale, string cancellationReason = "")
        {
            SaleItem = saleItem;
            Sale = sale;
            CancellationReason = cancellationReason;
        }
    }
}