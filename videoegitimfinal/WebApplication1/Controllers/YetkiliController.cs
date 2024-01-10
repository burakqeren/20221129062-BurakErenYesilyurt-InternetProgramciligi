using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using videoegitimfinal.Models;
using videoegitimfinal.ModelViews;

namespace videoegitimfinal.Controllers
{
    public class YetkiliController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;
        private readonly AppDbContext _appDbContext;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public YetkiliController(IConfiguration configuration, INotyfService notyfService = null, AppDbContext appDbContext = null, IFileProvider fileProvider = null, UserManager<AppUser> userManager = null, RoleManager<AppRole> roleManager = null, SignInManager<AppUser> signInManager = null)
        {
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
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Admin");
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
            var roleExist = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                var role = new AppRole { Name = "Admin" };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            _notyfService.Success("Kayıt Başarılı Bir Şekilde Gerçekleştirilidi");
            return RedirectToAction("Login", "Yetkili");
        }


    }
}
