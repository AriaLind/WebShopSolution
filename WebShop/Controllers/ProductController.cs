using Microsoft.AspNetCore.Mvc;
using WebShop.DataAccess;
using WebShop.Interfaces;

namespace WebShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private IUnitOfWork _unitOfWork;
    public ProductController(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext)
    {
        _unitOfWork = new UnitOfWork.UnitOfWork(applicationDbContext);
    }

    // Endpoint för att hämta alla produkter
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var result = await _unitOfWork.ProductRepository.GetAllAsync();

        return Ok(result);
    }

    // Endpoint för att hämta en specifik produkt
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        // Behöver använda repository via Unit of Work för att hämta en specifik produkt
        var result = await _unitOfWork.ProductRepository.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // Endpoint för att lägga till en ny produkt
    [HttpPost]
    public async Task<ActionResult> AddProduct(Product? product)
    {
        // Lägger till produkten via repository
        if (product == null) { return BadRequest(); }

        await _unitOfWork.ProductRepository.AddAsync(product);

        // Sparar förändringar
        await _unitOfWork.SaveChangesAsync();

        // Notifierar observatörer om att en ny produkt har lagts till
        _unitOfWork.NotifyProductAdded(product);

        return Ok();
    }

    // Endpoint för att uppdatera en produkt
    [HttpPut]
    public async Task<ActionResult> UpdateProduct(Product? product)
    {
        if (product == null) { return BadRequest(); }

        var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);

        if (existingProduct == null) { return BadRequest(); }

        // Uppdaterar produkten via repository
        await _unitOfWork.ProductRepository.UpdateAsync(product);

        // Sparar förändringar
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    // Endpoint för att ta bort en produkt
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        if (id <= 0) { return BadRequest(); }

        var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(id);

        if (existingProduct == null) { return BadRequest(); }

        // Tar bort produkten via repository
        await _unitOfWork.ProductRepository.DeleteAsync(id);

        // Sparar förändringar
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }
}