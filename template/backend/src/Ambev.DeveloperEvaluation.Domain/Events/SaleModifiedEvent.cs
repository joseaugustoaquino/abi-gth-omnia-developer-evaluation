using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    /// <summary>
    /// Event raised when a sale is modified
    /// </summary>
    public class SaleModifiedEvent
    {
        /// <summary>
        /// Gets the sale that was modified
        /// </summary>
        public Sale Sale { get; }

        /// <summary>
        /// Gets the original sale before modification
        /// </summary>
        public Sale OriginalSale { get; }

        /// <summary>
        /// Initializes a new instance of SaleModifiedEvent
        /// </summary>
        /// <param name="sale">The modified sale</param>
        /// <param name="originalSale">The original sale before modification</param>
        public SaleModifiedEvent(Sale sale, Sale originalSale)
        {
            Sale = sale;
            OriginalSale = originalSale;
        }
    }
}