using ASP.NET_Classwork.Models;
using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
