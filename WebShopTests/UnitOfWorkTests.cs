using DataAccess;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Moq;
using WebShop;
using WebShop.Interfaces;
using WebShop.Notifications;

namespace WebShopTests;

public class UnitOfWorkTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public UnitOfWorkTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _unitOfWorkMock
            .Setup(uow => uow.ProductRepository)
            .Returns(_productRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.NotifyProductAdded(It.IsAny<Product>()));

        _productRepositoryMock
            .Setup(pr => pr.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new Product { Id = id });

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public void NotifyProductAdded_CallsObserverUpdate()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };

        var mockObserver = new Mock<INotificationObserver>();

        var productSubject = new ProductSubject();
        productSubject.Attach(mockObserver.Object);

        _unitOfWorkMock
            .Setup(uow => uow.NotifyProductAdded(It.IsAny<Product>()))
            .Callback<Product>(p => productSubject.Notify(p));

        // Act
        _unitOfWorkMock.Object.NotifyProductAdded(product);

        // Assert
        mockObserver.Verify(o => o.Update(product), Times.Once);
    }


    [Fact]
    public void UnitOfWork_ShouldReturnProductRepository()
    {
        // Arrange

        // Act
        var productRepository = _unitOfWorkMock.Object.ProductRepository;

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

    [Fact]
    public void UnitOfWork_SaveChanges_ShouldCommitChanges()
    {
        // Arrange


        // Act
        _unitOfWorkMock.Object.SaveChangesAsync();

        // Assert
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}