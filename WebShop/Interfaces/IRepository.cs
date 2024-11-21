namespace WebShop.Interfaces;

public interface IRepository<TId, TEntity> where TId : notnull
{
    Task<Product?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TId id);
}