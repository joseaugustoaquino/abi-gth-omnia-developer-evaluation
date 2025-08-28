using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    /// <summary>
    /// Event raised when a sale is cancelled
    /// </summary>
    public class SaleCancelledEvent
    {
        /// <summary>
        /// Gets the sale that was cancelled
        /// </summary>
        public Sale Sale { get; }

        /// <summary>
        /// Gets the reason for cancellation
        /// </summary>
        public string CancellationReason { get; }

        /// <summary>
        /// Initializes a new instance of SaleCancelledEvent
        /// </summary>
        /// <param name="sale">The sale that was cancelled</param>
        /// <param name="cancellationReason">The reason for cancellation</param>
        public SaleCancelledEvent(Sale sale, string cancellationReason = "")
        {
            Sale = sale;
            CancellationReason = cancellationReason;
        }
    }
}