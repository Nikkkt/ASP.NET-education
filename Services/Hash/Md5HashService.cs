using System.Security.Cryptography;
using System.Text;

namespace ASP.NET_Classwork.Services.Hash
{
    public class Md5HashService : IHashService
    {
        public String Digest(String input)
        {
            return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input)));
        }
    }
}
