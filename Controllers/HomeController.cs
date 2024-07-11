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
        // ������� �������� - _logger
        private readonly ILogger<HomeController> _logger;

        // ��������� �� (���-)�����
        private readonly IHashService _hashService;

        private readonly IOtpService _otpService;

        private readonly IFileNameService _fileNameService;

        public HomeController(ILogger<HomeController> logger, IHashService hashService, IOtpService otpService, IFileNameService fileNameService)
        {
            _logger = logger;
            _hashService = hashService;
            _otpService = otpService;
            _fileNameService = fileNameService;
            // �������� ����� ����������� - ������� �������������� ������
            // ��������� ����� (��������) ������ ��������� ������������ � ��� ��������� �� ����� �������� ��'���� (��������) �����
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

            // �� ������� ���������� �� � ��������� ���� (redirect)
            if (HttpContext.Session.Keys.Contains("signup-data"))
            {
                // � ��� - �� ��������, ���������� ���
                var formModel = JsonSerializer.Deserialize<SignUpFormModel>(HttpContext.Session.GetString("signup-data")!)!;

                model.FormModel = formModel;
                model.ValidationErrors = _Validate(formModel);

                ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";

                // ��������� ��� � ���, ��� �������� ���������� ����������
                HttpContext.Session.Remove("signup-data");
            }
            return View(model);
        }

        public IActionResult Demo([FromQuery(Name = "user-email")] String userEmail, [FromQuery(Name = "user-name")] String userName)
        {
            // ������ ����� �� �����
            // ������ 1: ����� ��������� action. �'�������� ���������� ����������� �� ����� ����
            // <input name="userName"/> ----- Demo(String userName)
            // ���� � HTML ���������������� �����, �� �������� � �# (user-name), �� �������� ������� [From...] �� ����������� ���� ����� �������� ����������

            // ������ 1 ���������������, ���� ������� ��������� �������� (1-2)
            // ����� �������������� ����� - ������������ �������
            ViewData["data"] = $"email: {userEmail}, name: {userName}";
            return View(); 
        }

        public IActionResult RegUser(SignUpFormModel formModel) {
            HttpContext.Session.SetString("signup-data", JsonSerializer.Serialize(formModel));
            return RedirectToAction(nameof(SignUp));

            // ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";
            // return View("Demo");
            // ��������: ���� ������� ���������� ����� �������� �����, �� �� ��������� � �������
            // �) ���� �����������, �� ��� �� �� ��������
            // �) �������� ������ ��� �����, �� ���� ��������� �� ���������� ����� � ��, �����, ����

            // г�����: "�������� �����" - ������������� ������ �� �����'����������� �����

            // Client (Browser)                        Server (ASP)
            // [form]----------- POST RegUser -------------> [form]---Session
            // <---------------- 302 SignUp ----------------           |
            // ----------------- GET SignUp --------------->           |
            // <--------------------HTML------------------------- ����������
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
            res[nameof(model.Name)] = String.IsNullOrEmpty(model.Name) ? "�� ����������� ������ ����" : nameRegex.IsMatch(model.Name) ? null : "������ �������� ��'�";

            res[nameof(model.Description)] = String.IsNullOrEmpty(model.Description) ? "�� ����������� ������ ����" : null;

            res[nameof(model.Price)] = model.Price < 0 ? "ֳ�� �� ���� ���� ����� �� 0" : null;

            res[nameof(model.Amount)] = model.Amount < 0 ? "ʳ������ �� ���� ���� ����� �� 0" : null;

            return res;
        }

        private Dictionary<String, String?> _Validate(SignUpFormModel model)
        {
            // �������� - �� �������� ����� �� ���������� ������ ��������
            // ��������� �������� - {
            //                          "UserEmail": null,           null - ��������� ������ ��������
            //                          "UserName": "too short"      �������� - ����������� ��� �������
            //                       }
            Dictionary<String, String?> res = new();
            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            res[nameof(model.UserEmail)] = String.IsNullOrEmpty(model.UserEmail) ? "�� ����������� ������ ����" : emailRegex.IsMatch(model.UserEmail) ? null : "������ �������� ������";

            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            res[nameof(model.UserName)] = String.IsNullOrEmpty(model.UserName) ? "�� ����������� ������ ����" : nameRegex.IsMatch(model.UserName) ? null : "������ �������� ��'�";

            if (String.IsNullOrEmpty(model.UserPassword)) {
                res[nameof(model.UserPassword)] = "�� ����������� ������ ����";
            }
            else if (model.UserPassword.Length < 3)
            {
                res[nameof(model.UserPassword)] = "������ �� ���� �� �������� �� 3 �������";
            }
            else  
            {
                List<String> parts = [];
                if (!Regex.IsMatch(model.UserPassword, @"\d")) 
                { 
                    parts.Add(" ���� �����"); 
                }
                if (!Regex.IsMatch(model.UserPassword, @"\D"))
                {
                    parts.Add(" ���� �����");
                }
                if (!Regex.IsMatch(model.UserPassword, @"\W"))
                {
                    parts.Add(" ���� ����������");
                }
                if (parts.Count > 0)
                {
                    res[nameof(model.UserPassword)] = "������ ������� ������ ����������" + String.Join(',', parts);
                }
                else
                {
                    res[nameof(model.UserPassword)] = null;
                }
            }

            res[nameof(model.UserRepeat)] = String.IsNullOrEmpty(model.UserRepeat) ? "�� ����������� ������ ����" : model.UserPassword == model.UserRepeat ? null : "����� �� ���������";

            res[nameof(model.isAgree)] = model.isAgree ? null : "��������� �������� ������� �����";

            return res;
        }
    }
}
