using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ASP.NET_Classwork.Models.Home
{
    public class SignUpFormModel
    {
        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null;

        [FromForm(Name = "user-name")]
        public String UserName { get; set; } = null;

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; } = null;

        [FromForm(Name = "user-repeat")]
        public String UserRepeat { get; set; } = null;

        [JsonIgnore]
        [FromForm(Name = "user-avatar")]
        public IFormFile UserAvatar { get; set; } = null;

        [FromForm(Name = "is-agree")]
        public Boolean isAgree { get; set; } = false;

        [FromForm(Name = "user-birthday")]
        public String UserBirthday { get; set; } = null;
    }
}

// Моделі в ASP - це класи, за допомогою яких реалізується передача комплексних даних (набору даних).
// В інших системах для цього вживають термін DTO (Data Transfer Object)

// Розрізняють моделі форм (FormModel) та моделі представлень (PageModel)
