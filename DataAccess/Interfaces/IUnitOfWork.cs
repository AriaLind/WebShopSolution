using DataAccess.Entities;

namespace DataAccess.Interfaces;

// Gränssnitt för Unit of Work
public interface IUnitOfWork
{
    // Repository för produkter
    // Sparar förändringar (om du använder en databas)
    IProductRepository ProductRepository { get; }
    void NotifyProductAdded(Product product); // Notifierar observatörer om ny produkt
    Task SaveChangesAsync();
}