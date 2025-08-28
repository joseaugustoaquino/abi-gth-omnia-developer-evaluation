using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="request">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the update operation</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        // Check if sale is cancelled
        if (existingSale.Status == Domain.Enums.SaleStatus.Cancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        // Update sale properties
        existingSale.CustomerName = request.CustomerName;
        existingSale.BranchName = request.BranchName;
        existingSale.UpdatedAt = DateTime.UtcNow;

        // Clear existing items and add new ones
        existingSale.Items.Clear();
        foreach (var itemCommand in request.Items)
        {
            var saleItem = new SaleItem
            {
                Id = itemCommand.Id ?? Guid.NewGuid(),
                SaleId = existingSale.Id,
                ProductId = itemCommand.ProductId,
                ProductName = itemCommand.ProductName,
                Quantity = itemCommand.Quantity,
                UnitPrice = itemCommand.UnitPrice,
                DiscountPercentage = itemCommand.DiscountPercentage
            };
            
            saleItem.CalculateTotalAmount();
            existingSale.Items.Add(saleItem);
        }

        // Apply discount rules and recalculate totals
        existingSale.CalculateTotalAmount();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}