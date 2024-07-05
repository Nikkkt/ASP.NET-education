namespace ASP.NET_Classwork.Services.KDF
{
    public interface IKdfService
    {
        String DerivedKey(String password, String salt);
    }
}
