using Microsoft.EntityFrameworkCore;
using WebShop.DataAccess;
using WebShop.Interfaces;

namespace WebShop.Repositories;

public class ProductRepository(ApplicationDbContext? applicationDbContext) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        if (applicationDbContext == null)
        {
            throw new ArgumentNullException(nameof(applicationDbContext));
        }

        if (id <= 0)
        {
            return null;
        }

        var product = await applicationDbContext.Products.FindAsync(id);

        return product ?? null;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        if (applicationDbContext == null)
        {
            throw new ArgumentNullException(nameof(applicationDbContext));
        }

        return await applicationDbContext.Products.ToListAsync();
    }

    public async Task AddAsync(Product entity)
    {
        if (applicationDbContext == null)
        {
            throw new ArgumentNullException(nameof(applicationDbContext));
        }

        if (entity.Name == null)
        {
            return;
        }

        await applicationDbContext.Products.AddAsync(entity);
    }

    public async Task UpdateAsync(Product? entity)
    {
        if (applicationDbContext == null)
        {
            throw new ArgumentNullException(nameof(applicationDbContext));
        }

        if (entity?.Name == null || entity.Id <= 0)
        {
            return;
        }

        var existingProduct = await applicationDbContext.Products.FindAsync(entity.Id);

        if (existingProduct == null)
        {
            return;
        }

        existingProduct.Name = entity.Name;
    }


    public async Task DeleteAsync(int id)
    {
        if (applicationDbContext == null)
        {
            throw new ArgumentNullException(nameof(applicationDbContext));
        }

        var product = await applicationDbContext.Products.FindAsync(id);

        if (product == null)
        {
            return;
        }

        applicationDbContext.Products.Remove(product);
    }
}