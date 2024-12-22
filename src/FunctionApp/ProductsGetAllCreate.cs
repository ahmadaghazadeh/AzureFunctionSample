using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace FunctionApp
{
    public class ProductsGetAllCreate
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;

        public ProductsGetAllCreate(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<ProductsGetAllCreate>();
        }

        [Function("ProductsGetAllCreate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "products")]
            HttpRequestData req)
        {
            if (req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new CreatedResult("/products", product);
            }

            var products = await _context.Products.ToListAsync();

            return new OkObjectResult(products);
        }
    }
}