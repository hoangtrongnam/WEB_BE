using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using WEB_BANHANG_BE.Persistence;
using WEB_BANHANG_BE.Models;

namespace WebApi.Services.ProductService
{
    public interface IProductService
    {
        List<ProductModel> GetListProduct();
        ProductModel FindProduct(int productID);
        int DeleteProduct(int productID);
        int UpdateProduct(ProductModel product);
        int UpdateProduct(int productID);
    }
    public class ProductService : DbConnectionRepositoryBase, IProductService
    {
        private readonly IConfiguration _config;
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
            string sql = "SELECT product_id, name, price FROM product where active =1";
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
                    Product.Product_id = int.Parse(dr["product_id"].ToString() ?? "0");
                    Product.name = dr["name"].ToString();
                    Product.price = decimal.Parse(dr["price"].ToString() ?? "0"); ;
                    // set other properties of the modal as needed
                    lstProduct.Add(Product);
                }
                con.Close();
            }
            return lstProduct;
        }

        public ProductModel FindProduct(int product_id)
        {
            //var connection ="DATA SOURCE=localhost/orcl;User Id=SANG;Password=ABC123";
            var connection = _conn.ConnectionString;
            //Console.WriteLine(connection);
            List<ProductModel> lstProduct = new List<ProductModel>();
            string sql = "SELECT product_id, name, price FROM product where product_id = :product_id and active =1";
            ProductModel Product = new ProductModel();
            using (OracleConnection con = new OracleConnection(connection))
            {
                con.Open();
                OracleCommand cmd = new OracleCommand(sql, con);
                cmd.Parameters.Add(":product_id", product_id);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Product.Product_id = int.Parse(dr["product_id"].ToString() ?? "0");
                    Product.name = dr["name"].ToString();
                    Product.price = decimal.Parse(dr["price"].ToString() ?? "0"); ;
                }
                con.Close();
            }
            return Product;
        }

        public int DeleteProduct(int product_id)
        {
            var product = FindProduct(product_id);
            if (product.Product_id is null)
            {
                return 0;
            }
            else
            {
                var connection = _conn.ConnectionString;
                //Console.WriteLine(connection);
                List<ProductModel> lstProduct = new List<ProductModel>();
                string sql = "DELETE FROM product where product_id = :product_id";
                ProductModel Product = new ProductModel();
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    OracleCommand cmd = new OracleCommand(sql, con);
                    cmd.Parameters.Add(":product_id", product_id);

                    //Thực thi câu lệnh DELETE và kiểm tra số lượng bản ghi bị ảnh hưởng
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Không có bản ghi nào được xóa.");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("Đã xóa {0} bản ghi.", rowsAffected);
                        return 1;
                    }
                }
            }
        }

        public int UpdateProduct(int productID)
        {
            var product = FindProduct(productID);

            if (product.Product_id is null)
            {
                return 0;
            }
            else
            {
                var connection = _conn.ConnectionString;
                //Console.WriteLine(connection);
                string sql = "UPDATE product SET active = 0 where product_id = :product_id";
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    //Thiết lập tham số cho câu lệnh UPDATE
                    OracleCommand cmd = new OracleCommand(sql, con);

                    cmd.Parameters.Add(":product_id", productID);

                    //Thực thi câu lệnh UPDATE và kiểm tra số lượng bản ghi bị ảnh hưởng
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Không có bản ghi nào được cập nhật.");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("Đã cập nhật {0} bản ghi.", rowsAffected);
                        return 1;
                    }
                }
            }
        }
        public int UpdateProduct(ProductModel item)
        {
            var product = FindProduct(item.Product_id ?? 0);

            if (product.Product_id is null)
            {
                return 0;
            }
            else
            {
                var connection = _conn.ConnectionString;
                //Console.WriteLine(connection);
                string sql = "UPDATE product SET name = :name, price = :price where product_id = :product_id and active =1";
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    //Thiết lập tham số cho câu lệnh UPDATE
                    OracleCommand cmd = new OracleCommand(sql, con);

                    cmd.Parameters.Add(":name", item.name);
                    cmd.Parameters.Add(":price", item.price);
                    cmd.Parameters.Add(":product_id", item.Product_id);

                    //Thực thi câu lệnh UPDATE và kiểm tra số lượng bản ghi bị ảnh hưởng
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Không có bản ghi nào được cập nhật.");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("Đã cập nhật {0} bản ghi.", rowsAffected);
                        return 1;
                    }
                }
            }
        }
    }
}
