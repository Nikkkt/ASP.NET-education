using System.Reflection.Metadata;

namespace ASP.NET_Classwork.Models.Home
{
    public class SignUpPageModel
    {
        public SignUpFormModel? FormModel { get; set; }
        public Dictionary<String, String?>? ValidationErrors { get; set; }
    }
}
