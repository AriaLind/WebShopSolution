namespace WebShop.Interfaces;

// Gränssnitt för produktrepositoryt enligt Repository Pattern
public interface IProductRepository : IRepository<int, Product>
{
}