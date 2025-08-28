using AutoMapper;
using FluentValidation;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateSaleSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            CustomerName = "Updated Customer",
            BranchName = "Updated Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Updated Product",
                    Quantity = 3,
                    UnitPrice = 150m
                }
            }
        };

        var existingSale = new Sale
        {
            Id = saleId,
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now.AddDays(-1),
            CustomerName = "Original Customer",
            BranchName = "Original Branch",
            Items = new List<SaleItem>()
        };

        var updatedSale = new Sale
        {
            Id = saleId,
            SaleNumber = "SALE-001",
            SaleDate = existingSale.SaleDate,
            CustomerName = command.CustomerName,
            BranchName = command.BranchName,
            TotalAmount = 450m // 3 * 150 (no discount for less than 4 items)
        };

        var expectedResult = new UpdateSaleResult
        {
            Id = updatedSale.Id,
            TotalAmount = updatedSale.TotalAmount
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResult.Id);
        result.TotalAmount.Should().Be(expectedResult.TotalAmount);

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<UpdateSaleResult>(updatedSale);
    }

    [Fact]
    public async Task Handle_SaleNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            CustomerName = "Updated Customer",
            BranchName = "Updated Branch"
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty, // Invalid - empty ID
            CustomerName = "Updated Customer",
            BranchName = "Updated Branch"
        };

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        await _saleRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}