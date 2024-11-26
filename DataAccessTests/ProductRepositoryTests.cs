using DataAccess;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataAccessTests;

public class ProductRepositoryTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly List<Product> _products = [];

    public ProductRepositoryTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();

        _productRepositoryMock.Setup(pr => pr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Product { Id = 1, Name = "Test" });

        _productRepositoryMock.Setup(pr => pr.GetAllAsync()).ReturnsAsync(_products);

        _productRepositoryMock
            .Setup(pr => pr.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(product =>
            {
                product.Id = _products.Count + 1;
                _products.Add(product);
            })
            .Returns(Task.CompletedTask);

        _productRepositoryMock.Setup(pr => pr.UpdateAsync(It.IsAny<Product>())).Callback<Product>(product =>
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                _products[index] = product;
            }
        });

        _productRepositoryMock.Setup(pr => pr.DeleteAsync(It.IsAny<int>())).Callback<int>(id =>
        {
            var product = _products.Find(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        });
    }

    [Fact]
    public async Task AddProduct_ShouldAddProductToDatabase()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };

        // Act
        await _productRepositoryMock.Object.AddAsync(product);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddProduct_WhenNameIsNull_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 2, Name = null };

        // Act
        await _productRepositoryMock.Object.AddAsync(product);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProduct_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var product = new Product { Id = 3, Name = "Test" };


        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        product.Name = "Updated";

        await _productRepositoryMock.Object.UpdateAsync(product);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateProduct_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 4, Name = "NonExistent" };

        // Act
        product.Name = "Updated";
        await _productRepositoryMock.Object.UpdateAsync(product);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldDeleteProductFromDatabase()
    {
        // Arrange
        var product = new Product { Id = 5, Name = "Test" };

        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        await _productRepositoryMock.Object.DeleteAsync(product.Id);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProduct_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 6, Name = "NonExistent" };

        // Act
        await _productRepositoryMock.Object.DeleteAsync(product.Id);

        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 7, Name = "Test1" },
            new() { Id = 8, Name = "Test2" },
            new() { Id = 9, Name = "Test3" }
        };

        foreach (var product in products)
        {
            await _productRepositoryMock.Object.AddAsync(product);
        }

        // Act
        var result = await _productRepositoryMock.Object.GetAllAsync();

        // Assert
        Assert.Equal(products.Count, result.Count());
    }

    [Fact]
    public async Task GetAllProducts_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange

        // Act
        var result = await _productRepositoryMock.Object.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange

        // Act
        var result = await _productRepositoryMock.Object.GetByIdAsync(10);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 11, Name = "Test" };

        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task GetById_ExistingProductWithNullName_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 12, Name = null };

        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Null(result.Name);
    }

    [Fact]
    public async Task GetById_ExistingProductWithEmptyName_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 13, Name = string.Empty };

        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(string.Empty, result.Name);
    }

    [Fact]
    public async Task GetById_ExistingProductWithWhitespaceName_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 14, Name = " " };

        await _productRepositoryMock.Object.AddAsync(product);

        // Act
        var result = await _productRepositoryMock.Object.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(" ", result.Name);
    }
}