using DataAccess.Entities;

namespace WebShop.Interfaces;

public interface INotificationObserver
{
    void Update(Product product); // Metod som kallas när en ny produkt läggs till
}