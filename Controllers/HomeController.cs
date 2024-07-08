using ASP.NET_Classwork.Models;
using ASP.NET_Classwork.Models.Home;
using ASP.NET_Classwork.Models.Product;
using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

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
    }
}
