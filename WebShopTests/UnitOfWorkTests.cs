using Microsoft.EntityFrameworkCore;
using Moq;
using WebShop;
using WebShop.DataAccess;
using WebShop.Interfaces;
using WebShop.Notifications;
using WebShop.UnitOfWork;

namespace WebShopTests;

public class UnitOfWorkTests
{
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
}