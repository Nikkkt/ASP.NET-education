namespace ASP.NET_Classwork.Data.Entities
{
    public class Token
    {
        public Guid     Id        { get; set; }
        public Guid     UserId    { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
