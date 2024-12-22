using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp
{
    public class ProductsGetByIdUpdateDelete
    {
        private readonly ILogger<ProductsGetByIdUpdateDelete> _logger;
        private readonly AppDbContext _context;

        public ProductsGetByIdUpdateDelete(ILogger<ProductsGetByIdUpdateDelete> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("ProductsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete",
                Route = "products/{id}")]
            HttpRequestData req, int id)
        {
            if (req.Method == HttpMethods.Get)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id.ToString());
                if (product == null) return new NotFoundResult();
                return new OkObjectResult(product);
            }

            else if (req.Method == HttpMethods.Put)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                product.Id = id.ToString();
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return new OkObjectResult(product);
            }
            else
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id.ToString());
                if (product == null) return new NotFoundResult();

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
        }
    }
}