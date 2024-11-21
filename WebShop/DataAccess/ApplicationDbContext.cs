using Microsoft.EntityFrameworkCore;

namespace WebShop.DataAccess;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
}