using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Models;
using ASP.NET_Classwork.Models.Home;
using ASP.NET_Classwork.Models.Product;
using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.KDF;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.AspNetCore.Http;
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

        private readonly DataContext _dataContext;

        private readonly IKdfService _kdfService;

        private String fileErrorKey = "file-error";
        private String fileNameKey = "file-name";

        private String productFileErrorKey = "file-error";
        private String productFileNameKey = "file-name";

        public HomeController(ILogger<HomeController> logger, IHashService hashService, IOtpService otpService, IFileNameService fileNameService, DataContext dataContext, IKdfService kdfService)
        {
            _logger = logger;
            _hashService = hashService;
            _otpService = otpService;
            _fileNameService = fileNameService;
            _dataContext = dataContext;
            _kdfService = kdfService;
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

                ////////////
                if (model.ValidationErrors.Where(p => p.Value != null).Count() == 0)
                {
                    // ���� ������� �������� - �������� � ��
                    String salt = _hashService.Digest(_fileNameService.GenerateFileName(20));
                    _dataContext.Users.Add(new() 
                    {
                        Id = Guid.NewGuid(),
                        Name = formModel.UserName,
                        Email = formModel.UserEmail,
                        Salt = salt,
                        Dk = _kdfService.DerivedKey(formModel.UserPassword, salt),
                        Registered = DateTime.Now,
                        Avatar = HttpContext.Session.GetString(fileNameKey)
                    });
                    _dataContext.SaveChanges();
                }
                ////////////

                ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";

                // ��'� ������������� ����� (��������) ����� ����������� � ���
                if (HttpContext.Session.Keys.Contains(fileNameKey))
                {
                    ViewData["avatar"] = HttpContext.Session.GetString(fileNameKey);
                    HttpContext.Session.Remove(fileNameKey);
                }

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

            if (formModel.UserAvatar != null)
            {
                // 1. ³��������� ���������� �����
                int dotPosition = formModel.UserAvatar.FileName.IndexOf(".");
                if (dotPosition == -1) // ���� ���������� �����
                {
                    HttpContext.Session.SetString(fileErrorKey, "����� ��� ���������� �� �����������");
                }
                else
                {
                    String ext = formModel.UserAvatar.FileName[dotPosition..];
                    // 2. ��������� ���������� �� ������ ����������
                    String[] extentions = [".jpg", ".png", ".bmp"];
                    if (!extentions.Contains(ext))
                    {
                        HttpContext.Session.SetString(fileErrorKey, "�� ���������� ���������� �����");
                    }
                    else
                    {
                        // 3. ���������� ��'� �����, ������������, �� �� �� ������������� ������� ����
                        String filename;
                        String path = "./Uploads/User/"; // "./wwwroot/img/upload/";
                        do
                        {
                            filename = new FileNameService().GenerateFileName(16) + ext;
                        } while (System.IO.File.Exists(path + filename));

                        // 4. �������� ����, �������� � �� ��'� �����
                        using Stream writer = new StreamWriter(path + filename).BaseStream;
                        formModel.UserAvatar.CopyTo(writer);

                        HttpContext.Session.SetString(fileNameKey, filename);
                    }
                }
            }
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

                if (model.ValidationErrors.Where(p => p.Value != null).Count() == 0)
                {
                    _dataContext.Products.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Name = formModel.Name,
                        Description = formModel.Description,
                        Price = formModel.Price,
                        Amount = formModel.Amount,
                        Picture = HttpContext.Session.GetString(fileNameKey)
                    });
                    _dataContext.SaveChanges();
                }

                ViewData["productData"] = $"name: {formModel.Name}, description: {formModel.Description}, price: {formModel.Price}, amount: {formModel.Amount}";

                if (HttpContext.Session.Keys.Contains(productFileNameKey))
                {
                    ViewData["picture"] = HttpContext.Session.GetString(productFileNameKey);
                    HttpContext.Session.Remove(productFileNameKey);
                }

                HttpContext.Session.Remove("product-data");
            }
            return View(model);
        }

        public IActionResult AddProduct(ProductFormModel formModel)
        {
            HttpContext.Session.SetString("product-data", JsonSerializer.Serialize(formModel));

            if (formModel.Picture != null)
            {
                int dotPosition = formModel.Picture.FileName.IndexOf(".");
                if (dotPosition == -1) HttpContext.Session.SetString(productFileErrorKey, "����� ��� ���������� �� �����������");
                else
                {
                    String ext = formModel.Picture.FileName[dotPosition..];
                    String[] extentions = [".jpg", ".png", ".bmp"];
                    if (!extentions.Contains(ext)) HttpContext.Session.SetString(productFileErrorKey, "�� ���������� ���������� �����");
                    else
                    {
                        String filename;
                        String path = "./Uploads/Product/";
                        do
                        {
                            filename = new FileNameService().GenerateFileName(16) + ext;
                        } while (System.IO.File.Exists(path + filename));

                        using Stream writer = new StreamWriter(path + filename).BaseStream;
                        formModel.Picture.CopyTo(writer);

                        HttpContext.Session.SetString(productFileNameKey, filename);
                    }
                }
            }

            return RedirectToAction(nameof(Product));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Download([FromRoute] String id) 
        {
            // id - ��������� � ������������� �����, ���� - ��'� �����
            String filename = $"./Uploads/User/{id}";
            if (System.IO.File.Exists(filename))
            {
                var stream = new StreamReader(filename).BaseStream;
                return File(stream, "image/png");
            }
            return NotFound();
        }

        public IActionResult DownloadProduct([FromRoute] String id)
        {
            // id - ��������� � ������������� �����, ���� - ��'� �����
            String filename = $"./Uploads/Product/{id}";
            if (System.IO.File.Exists(filename))
            {
                var stream = new StreamReader(filename).BaseStream;
                return File(stream, "image/png");
            }
            return NotFound();
        }

        private Dictionary<String, String?> _ValidateProduct(ProductFormModel model)
        {
            Dictionary<String, String?> res = new();

            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            res[nameof(model.Name)] = String.IsNullOrEmpty(model.Name) ? "�� ����������� ������ ����" : nameRegex.IsMatch(model.Name) ? null : "������ �������� ��'�";

            res[nameof(model.Description)] = String.IsNullOrEmpty(model.Description) ? "�� ����������� ������ ����" : null;

            res[nameof(model.Price)] = model.Price < 0 ? "ֳ�� �� ���� ���� ����� �� 0" : null;

            res[nameof(model.Amount)] = model.Amount < 0 ? "ʳ������ �� ���� ���� ����� �� 0" : null;

            if (HttpContext.Session.Keys.Contains(productFileErrorKey))
            {
                res[nameof(model.Picture)] = HttpContext.Session.GetString(productFileErrorKey);
                HttpContext.Session.Remove(productFileErrorKey);
            }

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

            // ���������� �������� ����� �������� � ���
            if (HttpContext.Session.Keys.Contains(fileErrorKey))
            {
                res[nameof(model.UserAvatar)] = HttpContext.Session.GetString(fileErrorKey);
                HttpContext.Session.Remove(fileErrorKey);
            }

            return res;
        }
    }
}
