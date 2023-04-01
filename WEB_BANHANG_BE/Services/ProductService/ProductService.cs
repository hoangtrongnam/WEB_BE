using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using WEB_BANHANG_BE.Persistence;
using WebApi.Models;

namespace WebApi.Services.ProductService
{
    public interface IProductService
    {
        List<ProductModel> GetListProduct();
    }
    public class ProductService : DbConnectionRepositoryBase, IProductService
    {
        private readonly IConfiguration _config;
        //private readonly IProductService _ProductService;
        public ProductService(IConfiguration configuration, IDbConnectionFactory dbConnectionFactory)
        : base(dbConnectionFactory)
        {
            _config = configuration;
        }

        public List<ProductModel> GetListProduct()
        {
            //var connection ="DATA SOURCE=localhost/orcl;User Id=SANG;Password=ABC123";
            var connection = _conn.ConnectionString;
            //Console.WriteLine(connection);
            List<ProductModel> lstProduct = new List<ProductModel>();
            string sql = "SELECT product_id, name, price FROM product";
            using (OracleConnection con = new OracleConnection(connection))
            {
                con.Open();

                // thực thi truy vấn và đọc dữ liệu
                OracleCommand cmd = new OracleCommand(sql, con);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //Console.WriteLine(dr["product_id"] + ", " + dr["name"] + ", " + dr["price"]);
                    ProductModel Product = new ProductModel();
                    Product.Product_id = dr["product_id"].ToString();
                    Product.name = dr["name"].ToString();
                    Product.price = dr["price"].ToString();
                    // set other properties of the modal as needed
                    lstProduct.Add(Product);
                }
                con.Close();
            }
            return lstProduct;
        }
    }
}
