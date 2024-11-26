using DataAccess.Entities;

namespace DataAccess.Interfaces;

public interface INotificationObserver
{
    void Update(Product product); // Metod som kallas när en ny produkt läggs till
}