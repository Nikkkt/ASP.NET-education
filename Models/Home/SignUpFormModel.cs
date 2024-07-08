using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Classwork.Models.Home
{
    public class SignUpFormModel
    {
        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null;

        [FromForm(Name = "user-name")]
        public String UserName { get; set; } = null;
    }
}

// Моделі в ASP - це класи, за допомогою яких реалізується передача комплексних даних (набору даних).
// В інших системах для цього вживають термін DTO (Data Transfer Object)

// Розрізняють моделі форм (FormModel) та моделі представлень (PageModel)
