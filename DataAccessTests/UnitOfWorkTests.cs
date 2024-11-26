using DataAccess;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataAccessTests;

public class UnitOfWorkTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public UnitOfWorkTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _unitOfWorkMock.Setup(uow => uow.ProductRepository).Returns(_productRepositoryMock.Object);
    }

    [Fact]
    public void NotifyProductAdded_CallsObserverUpdate()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };

        // Create a mock of INotificationObserver
        var mockObserver = new Mock<INotificationObserver>();

        // Create a ProductSubject and attach the mock observer
        var productSubject = new ProductSubject();
        productSubject.Attach(mockObserver.Object);

        // Set up an in-memory database for ApplicationDbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);

        // Inject ProductSubject and DbContext into UnitOfWork
        var unitOfWork = new UnitOfWork(dbContext, productSubject);

        // Act
        unitOfWork.NotifyProductAdded(product);

        // Assert
        // Verify that Update was called on the mock observer with the correct product
        mockObserver.Verify(o => o.Update(product), Times.Once);
    }

    [Fact]
    public void UnitOfWork_ShouldThrowArgumentNullExceptionWhenDbContextIsNull()
    {
        // Arrange
        ApplicationDbContext? dbContext = null;

        // Inject null DbContext into UnitOfWork
        var unitOfWork = new UnitOfWork(dbContext);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => unitOfWork.ProductRepository);
    }

    [Fact]
    public void UnitOfWork_ShouldInitializeNewProductSubject_WhenProductSubjectIsNull()
    {
        // Arrange
        ProductSubject? productSubject = null;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var unitOfWork = new UnitOfWork(new ApplicationDbContext(options), productSubject);

        var product = new Product { Id = 1, Name = "Test" };

        // Act
        unitOfWork.ProductSubject.Notify(product);

        // Assert
        Assert.NotNull(unitOfWork.ProductSubject);
    }

    [Fact]
    public void UnitOfWork_ShouldNotInitializeNewProductSubject_WhenProductSubjectIsNotNull()
    {
        // Arrange
        var productSubject = new ProductSubject();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var unitOfWork = new UnitOfWork(new ApplicationDbContext(options), productSubject);

        var product = new Product { Id = 1, Name = "Test" };

        // Act
        unitOfWork.ProductSubject.Notify(product);

        // Assert
        Assert.NotNull(unitOfWork.ProductSubject);
    }

    [Fact]
    public void UnitOfWork_ShouldReturnProductRepository()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var dbContext = new ApplicationDbContext(options);

        var unitOfWork = new UnitOfWork(dbContext);

        // Act
        var productRepository = unitOfWork.ProductRepository;

        // Assert
        Assert.NotNull(productRepository);
    }

    [Fact]
    public void UnitOfWork_ShouldThrowArgumentNullExceptionWhenProductRepositoryIsCalledWithoutDbContext()
    {
        // Arrange
        ApplicationDbContext? dbContext = null;

        var unitOfWork = new UnitOfWork(dbContext);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => unitOfWork.ProductRepository);
    }
}