using ASP.NET_Classwork.Models;
using ASP.NET_Classwork.Models.Home;
using ASP.NET_Classwork.Models.Product;
using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ASP.NET_Classwork.Controllers
{
    public class HomeController : Controller
    {
        // приклад інжекції - _logger
        private readonly ILogger<HomeController> _logger;

        // інжектуємо на (хеш-)сервіс
        private readonly IHashService _hashService;

        private readonly IOtpService _otpService;

        private readonly IFileNameService _fileNameService;

        public HomeController(ILogger<HomeController> logger, IHashService hashService, IOtpService otpService, IFileNameService fileNameService)
        {
            _logger = logger;
            _hashService = hashService;
            _otpService = otpService;
            _fileNameService = fileNameService;
            // Інжекція через конструктор - найбільш рекомендований варіант
            // Контейнер служб (інжектор) аналізує параметри конструктора і сам підставляє до нього необхідні об'єкти (інстанси) служб
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }
        
        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult UrlInfo()
        {
            return View();
        }

        public IActionResult Ioc()
        {
            ViewData["hash"] = _hashService.Digest("123");
            ViewData["hashCode"] = _hashService.GetHashCode();
            ViewData["password"] = _otpService.GeneratePassword();
            ViewData["fileName"] = _fileNameService.GenerateFileName(10);
            return View();
        }

        public IActionResult SignUp()
        {
            SignUpPageModel model = new SignUpPageModel();

            // На початку перевіряємо чи є збережена сесія (redirect)
            if (HttpContext.Session.Keys.Contains("signup-data"))
            {
                // є дані - це редирект, обробляємо дані
                var formModel = JsonSerializer.Deserialize<SignUpFormModel>(HttpContext.Session.GetString("signup-data")!)!;

                model.FormModel = formModel;
                model.ValidationErrors = _Validate(formModel);

                ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";

                // Видаляємо дані з сесії, щоб уникнути повторного оброблення
                HttpContext.Session.Remove("signup-data");
            }
            return View(model);
        }

        public IActionResult Demo([FromQuery(Name = "user-email")] String userEmail, [FromQuery(Name = "user-name")] String userName)
        {
            // Прийом даних від форми
            // Варіант 1: через параметри action. З'язування відбувається автоматично за збігом імен
            // <input name="userName"/> ----- Demo(String userName)
            // Якщо в HTML використовуються імена, які неможливі у С# (user-name), то додається атрибут [From...] із зазначенням імені перед потрібним параметром

            // Варіант 1 використовується, коли кількість параметрів невелика (1-2)
            // Більш рекомендований спосіб - використання моделей
            ViewData["data"] = $"email: {userEmail}, name: {userName}";
            return View(); 
        }

        public IActionResult RegUser(SignUpFormModel formModel) {
            HttpContext.Session.SetString("signup-data", JsonSerializer.Serialize(formModel));
            return RedirectToAction(nameof(SignUp));

            // ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";
            // return View("Demo");
            // Проблема: якщо сторінка побудована через передачу форми, то її оновлення у браузері
            // а) видає повідомлення, на яке ми не впливаємо
            // б) повторно передає дані форми, що може призвести до дублювання даних у бд, файлів, тощо

            // Рішення: "скидання даних" - переадресація відповіді із запам'ятовуванням даних

            // Client (Browser)                        Server (ASP)
            // [form]----------- POST RegUser -------------> [form]---Session
            // <---------------- 302 SignUp ----------------           |
            // ----------------- GET SignUp --------------->           |
            // <--------------------HTML------------------------- оброблення
        }

        public IActionResult Product()
        {
            ProductPageModel model = new ProductPageModel();

            if (HttpContext.Session.Keys.Contains("product-data"))
            {
                var formModel = JsonSerializer.Deserialize<ProductFormModel>(HttpContext.Session.GetString("product-data")!)!;

                model.FormModel = formModel;
                model.ValidationErrors = _ValidateProduct(formModel);

                ViewData["productData"] = $"name: {formModel.Name}, description: {formModel.Description}, price: {formModel.Price}, amount: {formModel.Amount}";

                HttpContext.Session.Remove("product-data");
            }
            return View(model);
        }

        public IActionResult AddProduct(ProductFormModel formModel)
        {
            HttpContext.Session.SetString("product-data", JsonSerializer.Serialize(formModel));
            return RedirectToAction(nameof(Product));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Dictionary<String, String?> _ValidateProduct(ProductFormModel model)
        {
            Dictionary<String, String?> res = new();

            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            res[nameof(model.Name)] = String.IsNullOrEmpty(model.Name) ? "Не допускається порожнє поле" : nameRegex.IsMatch(model.Name) ? null : "Введіть коректне ім'я";

            res[nameof(model.Description)] = String.IsNullOrEmpty(model.Description) ? "Не допускається порожнє поле" : null;

            res[nameof(model.Price)] = model.Price < 0 ? "Ціна не може бути менша за 0" : null;

            res[nameof(model.Amount)] = model.Amount < 0 ? "Кількість не може бути менша за 0" : null;

            return res;
        }

        private Dictionary<String, String?> _Validate(SignUpFormModel model)
        {
            // Валідація - це перевірка даних на відповідність певним шаблонам
            // Результат валідації - {
            //                          "UserEmail": null,           null - результат успішної валідації
            //                          "UserName": "too short"      значення - повідомлення про помилку
            //                       }
            Dictionary<String, String?> res = new();
            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            res[nameof(model.UserEmail)] = String.IsNullOrEmpty(model.UserEmail) ? "Не допускається порожнє поле" : emailRegex.IsMatch(model.UserEmail) ? null : "Введіть коректну адресу";

            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            res[nameof(model.UserName)] = String.IsNullOrEmpty(model.UserName) ? "Не допускається порожнє поле" : nameRegex.IsMatch(model.UserName) ? null : "Введіть коректне ім'я";

            if (String.IsNullOrEmpty(model.UserPassword)) {
                res[nameof(model.UserPassword)] = "Не допускається порожнє поле";
            }
            else if (model.UserPassword.Length < 3)
            {
                res[nameof(model.UserPassword)] = "Пароль має бути не коротшим за 3 символи";
            }
            else  
            {
                List<String> parts = [];
                if (!Regex.IsMatch(model.UserPassword, @"\d")) 
                { 
                    parts.Add(" одну цифру"); 
                }
                if (!Regex.IsMatch(model.UserPassword, @"\D"))
                {
                    parts.Add(" одну літеру");
                }
                if (!Regex.IsMatch(model.UserPassword, @"\W"))
                {
                    parts.Add(" один спецсимвол");
                }
                if (parts.Count > 0)
                {
                    res[nameof(model.UserPassword)] = "Пароль повинен містити щонайменше" + String.Join(',', parts);
                }
                else
                {
                    res[nameof(model.UserPassword)] = null;
                }
            }

            res[nameof(model.UserRepeat)] = String.IsNullOrEmpty(model.UserRepeat) ? "Не допускається порожнє поле" : model.UserPassword == model.UserRepeat ? null : "Паролі не збігаються";

            res[nameof(model.isAgree)] = model.isAgree ? null : "Необхідно прийняти правила сайту";

            return res;
        }
    }
}
