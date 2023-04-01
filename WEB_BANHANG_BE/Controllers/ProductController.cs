using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using WebApi.Services.ProductService;

namespace WEB_BANHANG_BE.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    //private readonly OracleConnection _connection;
    private readonly IConfiguration _config;
    private readonly ILogger<ProductController> _logger;
     private readonly IProductService _productService;

    public ProductController(IConfiguration config, ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _config = config;
        _productService = productService;
    }


    [HttpGet]
    public IActionResult GetListProduct()
    {
        return Ok(_productService.GetListProduct());
    }
}
