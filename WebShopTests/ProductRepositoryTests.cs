using Microsoft.EntityFrameworkCore;
using WebShop;
using WebShop.DataAccess;
using WebShop.Repositories;

namespace WebShopTests;

public class ProductRepositoryTests
{
    [Fact]
    public async Task AddProduct_ShouldAddProductToDatabase()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        // Act
        await productRepository.AddAsync(product);

        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddProduct_WhenNameIsNull_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 2, Name = null };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        // Act
        await productRepository.AddAsync(product);

        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProduct_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var product = new Product { Id = 3, Name = "Test" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await productRepository.AddAsync(product);

        // Act

        product.Name = "Updated";

        await productRepository.UpdateAsync(product);

        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateProduct_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 4, Name = "NonExistent" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);
        var productRepository = new ProductRepository(dbContext);

        // Act
        product.Name = "Updated";
        await productRepository.UpdateAsync(product);

        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldDeleteProductFromDatabase()
    {
        // Arrange
        var product = new Product { Id = 5, Name = "Test" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await productRepository.AddAsync(product);

        // Act
        await productRepository.DeleteAsync(product.Id);

        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProduct_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var product = new Product { Id = 6, Name = "NonExistent" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        // Act
        await productRepository.DeleteAsync(product.Id);

        var result = await productRepository.GetByIdAsync(product.Id);

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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        foreach (var product in products)
        {
            await productRepository.AddAsync(product);
        }

        // Act
        var result = await productRepository.GetAllAsync();

        // Assert
        Assert.Equal(products.Count, result.Count());
    }

    [Fact]
    public async Task GetAllProducts_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        // Act
        var result = await productRepository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        // Act
        var result = await productRepository.GetByIdAsync(10);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 11, Name = "Test" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await productRepository.AddAsync(product);

        // Act
        var result = await productRepository.GetByIdAsync(product.Id);

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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await dbContext.AddAsync(product);

        // Act
        var result = await productRepository.GetByIdAsync(product.Id);

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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await productRepository.AddAsync(product);

        // Act
        var result = await productRepository.GetByIdAsync(product.Id);

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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ApplicationDbContext(options);

        var productRepository = new ProductRepository(dbContext);

        await productRepository.AddAsync(product);

        // Act
        var result = await productRepository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(" ", result.Name);
    }
}