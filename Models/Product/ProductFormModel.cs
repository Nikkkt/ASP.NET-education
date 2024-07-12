using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ASP.NET_Classwork.Models.Product
{
    public class ProductFormModel
    {
        [FromForm(Name = "product-name")]
        public String Name { get; set; } = null;

        [FromForm(Name = "product-description")]
        public String Description { get; set; } = null;
        
        [FromForm(Name = "product-price")]
        public int Price { get; set; } = 0;
        
        [FromForm(Name = "product-amount")]
        public int Amount { get; set; } = 0;

        [JsonIgnore]
        [FromForm(Name = "product-picture")]
        public IFormFile Picture { get; set; } = null;

    }
}
