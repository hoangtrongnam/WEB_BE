using Newtonsoft.Json;
namespace WebApi.Models
{
    public class ProductModel
    {
        [JsonProperty("product_id")]
        public string? Product_id { get; set; }

        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("price")]
        public string? price { get; set; }
    }
}
