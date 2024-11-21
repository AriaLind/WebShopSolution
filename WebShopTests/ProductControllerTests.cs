using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebShop;
using WebShop.Controllers;
using WebShop.DataAccess;
using WebShop.Interfaces;

namespace WebShopTests;

public class ProductControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        // Mock the IProductRepository
        _mockProductRepository = new Mock<IProductRepository>();

        // Set up mock repository to return a list of products when GetAllAsync is called
        _mockProductRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Product>
            {
                    new Product { Id = 1, Name = "Test1" },
                    new Product { Id = 2, Name = "Test2" },
                    new Product { Id = 3, Name = "Test3" }
            });

        // Set up mock repository to return a specific product when GetByIdAsync is called
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Product { Id = 1, Name = "Test1" });

        // Mock the IUnitOfWork to return the mock repository
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUnitOfWork.Setup(uow => uow.ProductRepository).Returns(_mockProductRepository.Object);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var dbContext = new ApplicationDbContext(options);

        // Create the controller using the mocked IUnitOfWork
        _controller = new ProductController(_mockUnitOfWork.Object, dbContext);
    }

    [Fact]
    public async Task GetProduct_ReturnsOkResult_WithAProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test1" };

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithAListOfProducts()
    {
        // Arrange

        // Act
        var result = await _controller.GetProducts();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddProduct_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Id = 4, Name = "Test4" };

        // Act
        var result = await _controller.AddProduct(product);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddProduct_ReturnsBadRequestResult_WhenProductIsNull()
    {
        // Arrange

        // Act
        var result = await _controller.AddProduct(null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "UpdatedProduct1" };

        // Act
        var result = await _controller.UpdateProduct(product);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequestResult_WhenProductIsNull()
    {
        // Arrange

        // Act
        var result = await _controller.UpdateProduct(null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequestResult_WhenProductIsNotFound()
    {
        // Arrange
        var product = new Product { Id = 7, Name = "Test7" };

        // Act
        var result = await _controller.UpdateProduct(product);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Id = 8, Name = "Test8" };

        // Act
        var result = await _controller.DeleteProduct(8);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsBadRequestResult_WhenProductIsNotFound()
    {
        // Arrange
        var product = new Product { Id = 9, Name = "Test9" };

        // Act
        var result = await _controller.DeleteProduct(product.Id);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsBadRequestResult_WhenProductIsLessThanZero()
    {
        // Arrange

        // Act
        var result = await _controller.DeleteProduct(-1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}