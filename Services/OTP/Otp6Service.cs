namespace ASP.NET_Classwork.Services.OTP
{
    public class Otp6Service : IOtpService
    {
        public String GeneratePassword()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
