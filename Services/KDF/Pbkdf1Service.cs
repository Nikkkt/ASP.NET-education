using ASP.NET_Classwork.Services.Hash;

namespace ASP.NET_Classwork.Services.KDF
{
    // Згідно з п. 5.1 RFC 2898
    public class Pbkdf1Service(IHashService hashService) : IKdfService
    {
        // Сервіси можуть інжектувати інші сервіси
        private readonly IHashService _hashService = hashService;
        public string DerivedKey(string password, string salt)
        {
            String t1 = _hashService.Digest(password + salt);
            String t2 = _hashService.Digest(t1);
            return t2;
        }
    }
}
