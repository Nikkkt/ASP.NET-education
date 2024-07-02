using System.Security.Cryptography;
using System.Text;

namespace ASP.NET_Classwork.Services.Hash
{
    public class ShaHashService : IHashService
    {
        public String Digest(String input)
        {
            return Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(input)));
        }
    }
}
