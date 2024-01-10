using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using videoegitimfinal.Models;
using videoegitimfinal.ModelViews;

namespace videoegitimfinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;
        private readonly AppDbContext _appDbContext;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, INotyfService notyfService = null, AppDbContext appDbContext = null, IFileProvider fileProvider = null, UserManager<AppUser> userManager = null, RoleManager<AppRole> roleManager = null, SignInManager<AppUser> signInManager = null)
        {
            _logger = logger;
            _configuration = configuration;
            _notyfService = notyfService;
            _appDbContext = appDbContext;
            _fileProvider = fileProvider;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var egitim = _appDbContext.Egitims.Select(x => new EgitimModelView()
            {
                Id = x.Id,
                KategoriAdı = x.kategoriAdı
            }).ToList();
            return View(egitim);
        }
        public IActionResult KategoriBul(int id)
        {
            var getir = _appDbContext.Videos
             .Where(x => x.EgitimId == id)
             .Select(y => new VideoViewModel
            {
                Name = y.Name,
                Description = y.Description,
                Id = y.Id,  
            })
            .ToList();

            var isim = _appDbContext.Egitims.Where(x => x.Id == id).Select(y => y.kategoriAdı).FirstOrDefault();
            var dersid = _appDbContext.Videos.Select(x => x.Id).FirstOrDefault();
            ViewBag.Isim = isim;
            ViewBag.dersid = dersid;
            return View(getir);
        }

        public IActionResult DersGetir(int Id)
        {
            var getir = _appDbContext.Videos.Where(x => x.Id == Id).Select(y => new VideoViewModel()
            {
                Name = y.Name,
                Description = y.Description,
                Kayıt = y.Kayıt

            }).ToList();
            return View(getir);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult contanct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult contanct(IletisimViewModel model)
        {
            var kaydet = new Iletisim();
            kaydet.Name = model.Name;
            kaydet.Email = model.Email;
            kaydet.Surname = model.Surname;
            kaydet.Message = model.Message;
            _appDbContext.Iletisims.Add(kaydet);
            _appDbContext.SaveChangesAsync();
            _notyfService.Success("Mesaj Başarıyla İletildi");
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModelView model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz Kullanıcı Adı veya Parola!");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Kullanıcı Girişi " + user.LockoutEnd + " kadar kısıtlanmıştır!");
                return View();
            }
            ModelState.AddModelError("", "Geçersiz Kullanıcı Adı veya Parola Başarısız Giriş Sayısı :" + await _userManager.GetAccessFailedCountAsync(user) + "/3");
            return View();

        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModelView model)
        {

            var identityResult = await _userManager.CreateAsync(new() { UserName = model.UserName, Email = model.Email, City = model.City, FullName = model.FullName }, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            var roleExist = await _roleManager.RoleExistsAsync("Ogrenci");
            if (!roleExist)
            {
                var role = new AppRole { Name = "Ogrenci" };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Ogrenci");
            _notyfService.Success("Kayıt Başarılı Bir Şekilde Gerçekleştirilidi");
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Privacy()
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