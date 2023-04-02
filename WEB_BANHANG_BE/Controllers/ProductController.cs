using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using WebApi.Services.ProductService;
using WEB_BANHANG_BE.Models;

namespace WEB_BANHANG_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
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


    [HttpGet("/GetListProduct")]
    public IActionResult GetListProduct()
    {
        // Lấy thông tin sản phẩm với ID được chỉ định
        var lstProduct = _productService.GetListProduct();

        // Nếu không tìm thấy sản phẩm, trả về mã lỗi 404
        if (lstProduct == null)
        {
            return NotFound();
        }

        return Ok(lstProduct);
    }

    [HttpGet("/FindProduct/{id}")]
    public IActionResult FindProduct(int id)
    {
        // Lấy thông tin sản phẩm với ID được chỉ định
        var product = _productService.FindProduct(id);
        // Nếu không tìm thấy sản phẩm, trả về mã lỗi 404
        if (product.Product_id is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        // Xóa sản phẩm với ID được chỉ định
        // code xóa sản phẩm khỏi cơ sở dữ liệu
        int result = _productService.UpdateProduct(id);
        if (result == 1)
        {
            return Ok("xoa thanh cong");
        }
        // Trả về mã lỗi 204 để cho biết sản phẩm đã được xóa thành công
        return NoContent();
    }

    [HttpPut()]
    public IActionResult UpdateProduct([FromBody] ProductModel product)
    {
        // Cập nhật thông tin sản phẩm với ID được chỉ định
        // code cập nhật sản phẩm trong cơ sở dữ liệu
        int result = _productService.UpdateProduct(product);
        if (result == 0)
        {
            return NoContent();
        }

        // Trả về thông tin sản phẩm đã cập nhật dưới dạng JSON
        return Ok(product);
    }
}
