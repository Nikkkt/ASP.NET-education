namespace ASP.NET_Classwork.Services.OTP
{
    public class Otp4Service : IOtpService
    {
        public String GeneratePassword()
        {
            return new Random().Next(1000, 9999).ToString();
        }
    }
}
