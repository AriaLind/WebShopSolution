using WebShop.DataAccess;
using WebShop.Interfaces;
using WebShop.Notifications;
using WebShop.Repositories;

namespace WebShop.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private ApplicationDbContext? _applicationDbContext;
    // Hämta produkter från repository
    private IProductRepository? _productRepository;

    private ProductSubject? _productSubject;

    public ProductSubject? ProductSubject
    {
        get
        {
            if (_productSubject is null)
            {
                _productSubject = new ProductSubject();

                _productSubject.Attach(new EmailNotification());
            }

            return _productSubject;
        }
    }

    public UnitOfWork(ApplicationDbContext? applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public UnitOfWork(ApplicationDbContext? applicationDbContext, ProductSubject? productSubject)
    {
        _applicationDbContext = applicationDbContext;
        _productSubject = productSubject;
    }

    public IProductRepository ProductRepository
    {
        get
        {
            if (_applicationDbContext is not null)
            {
                return _productRepository ??= new ProductRepository(_applicationDbContext);
            }
            throw new ArgumentNullException(nameof(_applicationDbContext));
        }
    }

    //Konstruktor används för tillfället av Observer pattern
    //public UnitOfWork(ProductSubject productSubject = null)
    //{
    //    Products = null;

    //    // Om inget ProductSubject injiceras, skapa ett nytt
    //    _productSubject = productSubject ?? new ProductSubject();

    //    // Registrera standardobservatörer
    //    _productSubject.Attach(new EmailNotification());
    //}

    public void NotifyProductAdded(Product product)
    {
        _productSubject?.Notify(product);
    }

    public async Task SaveChangesAsync()
    {
        if (_applicationDbContext != null) await _applicationDbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _applicationDbContext?.Dispose();
    }
}