using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Services.KDF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ASP.NET_Classwork.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IKdfService _kdfService;

        public AuthController(DataContext dataContext, IKdfService kdfService)
        {
            _dataContext = dataContext;
            _kdfService = kdfService;
        }

        [HttpGet]
        public object DoGet(String input, String password)
        {
            if (String.IsNullOrEmpty(input) || String.IsNullOrEmpty(password))
            {
                return new
                {
                    status = "Error",
                    code = 400,
                    message = "Email/Name/Birthday and password must not be empty"
                };
            }

            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            var birthdayRegex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])[-./](0[1-9]|1[0-2])[-./](\d{4})$");

            var user = emailRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Email == input) :
                nameRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Name == input) :
                birthdayRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Birthdate == Convert.ToDateTime(input)) :
                null;

            // Розшифрувати DK неможливо, тому повторюємо розрахунок DK з сіллю, що зберігається у користувача, та паролем, який був переданий
            if (user != null && _kdfService.DerivedKey(password, user.Salt) == user.Dk)
            {
                return new
                {
                    status = "Ok",
                    code = 200,
                    message = "Authenticated"
                };
            }
            else
            {
                return new
                {
                    status = "Reject",
                    code = 401,
                    message = "Credentials rejected"
                };
            }
        }
    }
}

// Контролери розрізняють MVC та API 
// MVC - різні адреси ведуть на різні дії (action)
//       /Home/Index -> Index()
//       /Home/Db    -> Db()
//
// API - різні методи запиту ведуть на різні дії 
//       GET  /api/auth  -> DoGet()
//       POST /api/auth  -> DoPost()
//       PUT  /api/auth  -> DoPut()