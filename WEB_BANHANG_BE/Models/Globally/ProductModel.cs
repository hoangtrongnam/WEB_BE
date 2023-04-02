using Newtonsoft.Json;
namespace WEB_BANHANG_BE.Models
{
    public class ProductModel
    {
        [JsonProperty("Product_id")]
        public int? Product_id { get; set; }

        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("price")]
        public decimal? price { get; set; }
    }
}
