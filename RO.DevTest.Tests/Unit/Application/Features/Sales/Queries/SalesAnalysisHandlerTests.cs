using Bogus;
using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries;
using RO.DevTest.Application.Features.Sale.Queries.SalesAnalysisQueries;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Tests.Unit.Application.Features.Sales.Queries;

public class SalesAnalysisHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly SalesAnalysisHandler _sut;
    private readonly Faker _faker;

    public SalesAnalysisHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _sut = new SalesAnalysisHandler(_saleRepositoryMock.Object);
        _faker = new Faker("pt_BR");
    }

    [Fact(DisplayName = "Deve calcular vendas e receita corretamente com dados fake")]
    public async Task Handle_DeveRetornarAnaliseCorretaComDadosFake()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = _faker.Commerce.ProductName();
        var precoUnitario = _faker.Random.Float(10, 100);
        var quantidade1 = _faker.Random.Int(1, 5);
        var quantidade2 = _faker.Random.Int(1, 5);


        var fakeSales = new List<Sale>
        {
            new()
            {
                DateSale = DateTime.UtcNow.AddDays(-1),
                Itens = new List<SaleItem>
                {
                    new()
                    {
                        ProductId = productId,
                        Amount = quantidade1,
                        UnitPrice = precoUnitario,
                        Product = new Product
                        {
                            Id = productId,
                            Name = productName
                        }
                    },
                    new()
                    {
                        ProductId = productId,
                        Amount = quantidade2,
                        UnitPrice = precoUnitario,
                        Product = new Product
                        {
                            Id = productId,
                            Name = productName
                        }
                    }
                }
            }
        };

        _saleRepositoryMock
            .Setup(repo => repo.GetSalesByPeriod(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(fakeSales);

        var request = new SalesAnalysisRequest
        {
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow
        };

        var expectedTotalSold = quantidade1 + quantidade2;
        var expectedRevenue = expectedTotalSold * precoUnitario;

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TotalSales.Should().Be(1);
        result.TotalRevenue.Should().BeApproximately(expectedRevenue, 0.01f);
        result.ProductRevenues.Should().ContainSingle(p =>
            p.ProductId == productId &&
            p.ProductName == productName &&
            p.TotalSold == expectedTotalSold &&
            Math.Abs(p.TotalRevenue - expectedRevenue) < 0.01f
        );
    }
}
