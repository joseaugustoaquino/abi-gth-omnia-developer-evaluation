using AutoMapper;
using FluentValidation;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute.ExceptionExtensions;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSaleSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerName = "Test Customer",
            BranchName = "Test Branch",
            TotalAmount = 500m
        };

        var expectedResult = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerName = sale.CustomerName,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResult.Id);
        result.SaleNumber.Should().Be(expectedResult.SaleNumber);
        result.CustomerName.Should().Be(expectedResult.CustomerName);
        result.BranchName.Should().Be(expectedResult.BranchName);
        result.TotalAmount.Should().Be(expectedResult.TotalAmount);

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleResult>(sale);
    }

    [Fact]
    public async Task Handle_SaleNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        _mapper.DidNotReceive().Map<GetSaleResult>(Arg.Any<Sale>());
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.Empty); // Invalid - empty ID

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        await _saleRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        _mapper.DidNotReceive().Map<GetSaleResult>(Arg.Any<Sale>());
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");
    }
}