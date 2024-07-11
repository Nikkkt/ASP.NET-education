namespace ASP.NET_Classwork.Models.Product
{
    public class ProductPageModel
    {
        public ProductFormModel? FormModel { get; set; }
        public Dictionary<String, String?>? ValidationErrors { get; set; }
    }
}
