namespace ASP.NET_Classwork.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        public String Description { get; set; } = null!;
        public int Price { get; set; }
        public int Amount { get; set; }
        public String? Picture { get; set; } = null!;
    }
}
