using Microsoft.AspNetCore.Mvc;

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
        
        [FromForm(Name = "is-agree")]
        public Boolean isAgree { get; set; } = false;
    }
}

// Моделі в ASP - це класи, за допомогою яких реалізується передача комплексних даних (набору даних).
// В інших системах для цього вживають термін DTO (Data Transfer Object)

// Розрізняють моделі форм (FormModel) та моделі представлень (PageModel)
