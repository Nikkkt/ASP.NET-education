namespace ASP.NET_Classwork.Data.Entities
{
    public class User
    {
        public Guid      Id         { get; set; }
        public String    Email      { get; set; } = null!;
        public String    Name       { get; set; } = null!;
        public DateTime? Birthdate  { get; set; }
        public DateTime  Registered { get; set; }

        //////////////////// Initial ///////////////////

        public String  Salt   { get; set; } = null!;
        public String  Dk     { get; set; } = null!;
        public String? Avatar { get; set; } = null!;
    }
}
