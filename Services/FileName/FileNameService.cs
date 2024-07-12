namespace ASP.NET_Classwork.Services.FileName
{
    public class FileNameService : IFileNameService
    {
        public string GenerateFileName(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyz" + "0123456789";
            Random random = new Random();

            String result = "";
            for (int i = 0; i < length; i++) result += allowedChars[random.Next(allowedChars.Length)];

            return result;
        }
    }
}
