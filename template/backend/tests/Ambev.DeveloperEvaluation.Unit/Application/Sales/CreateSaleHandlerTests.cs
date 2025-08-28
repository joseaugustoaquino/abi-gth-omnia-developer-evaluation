using AutoMapper;
using MediatR;
using FluentValidation;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute.ExceptionExtensions;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateSaleSuccessfully()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerName = command.CustomerName,
            BranchName = command.BranchName
        };

        var createdSale = new Sale
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerName = sale.CustomerName,
            BranchName = sale.BranchName,
            TotalAmount = 450m // 5 * 100 * 0.9 (10% discount)
        };

        var expectedResult = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalAmount = createdSale.TotalAmount
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResult.Id);
        result.SaleNumber.Should().Be(expectedResult.SaleNumber);
        result.TotalAmount.Should().Be(expectedResult.TotalAmount);

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<Sale>(command);
        _mapper.Received(1).Map<CreateSaleResult>(createdSale);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "", // Invalid - empty sale number
            SaleDate = DateTime.Now,
            CustomerName = "Test Customer",
            BranchName = "Test Branch"
        };

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<CreateSaleItemCommand>()
        };

        var sale = new Sale();
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");
    }
}