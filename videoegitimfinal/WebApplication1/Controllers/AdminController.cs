using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using videoegitimfinal.Models;
using videoegitimfinal.ModelViews;

namespace videoegitimfinal.Controllers
{
    [Authorize(Roles = "Ogretmen,Admin")]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;
        private readonly AppDbContext _appDbContext;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AdminController(IConfiguration configuration = null, INotyfService notyfService = null, AppDbContext appDbContext = null, IFileProvider fileProvider = null, UserManager<AppUser> userManager = null, RoleManager<AppRole> roleManager = null, SignInManager<AppUser> signInManager = null)
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

     
        public IActionResult Egitim() { 

            return View(); 
        }
        [HttpGet]
        public IActionResult EgitimGetir()
        {
            var egitim = _appDbContext.Egitims.Select(x => new EgitimModelView()
            {
                Id = x.Id,
                KategoriAdı = x.kategoriAdı
            }).ToList();
            return Json(egitim);

        }
        [HttpPost]
        public async Task<IActionResult> EgitimEkle(EgitimModelView model)
        {
            var egitim = new Egitim();
            egitim.kategoriAdı = model.KategoriAdı;
            _appDbContext.Egitims.Add(egitim);
            await _appDbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> EgitimGuncelle(EgitimModelView model)
        {

            var Egitim = _appDbContext.Egitims.SingleOrDefault(x => x.Id == model.Id);
            Egitim.kategoriAdı = model.KategoriAdı;
            _appDbContext.Egitims.Update(Egitim);
            await _appDbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> EgitimSil(EgitimModelView model)
        {
            var Egitim = _appDbContext.Egitims.FirstOrDefault(x => x.Id == model.Id);
            _appDbContext.Egitims.Remove(Egitim);
            await _appDbContext.SaveChangesAsync();
            return Json(new { success = true });

        }
        

        public async Task<IActionResult> Dersler()
        {
            var egitimler = _appDbContext.Videos.Select(x=>new VideoViewModel()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,   
            }).ToList();
            return View(egitimler);
        }
        [HttpGet]
        public async Task<IActionResult> Dersekle()
        {
            List<SelectListItem> EgitimListesi = (from x in _appDbContext.Egitims.ToList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.kategoriAdı,
                                                      Value = x.Id.ToString()
                                                  }).ToList();
            ViewBag.EgitimListesi = EgitimListesi;


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Dersekle(VideoViewModel models)
        {
            var rootfolder = _fileProvider.GetDirectoryContents("wwwroot");
            var VideoUrl = "-";
            if (models.Videolar.Length > 0 && models.Videolar != null)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(models.Videolar.FileName);
                var photoPath = Path.Combine(rootfolder.First(x => x.Name == "Videos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                models.Videolar.CopyTo(stream);
                VideoUrl = filename;
            }

            var VideoEkle = new Video();
            VideoEkle.Name = models.Name;
            VideoEkle.Description = models.Description;
            VideoEkle.Kayıt = VideoUrl;
            VideoEkle.EgitimId = models.EgitimId;
             _appDbContext.Videos.AddAsync(VideoEkle);
             _appDbContext.SaveChangesAsync();
            _notyfService.Success("Yeni Video Eklendi");
            return RedirectToAction("Dersler", "Admin");
        }
        
        public async  Task<IActionResult> DersSil(int id)
        {
            var ders = _appDbContext.Videos.Where(x=>x.Id == id).FirstOrDefault();
            _appDbContext.Videos.Remove(ders);
            _appDbContext.SaveChanges();
            return RedirectToAction("Dersler", "Admin");
        }
        [HttpGet]
        public IActionResult DersGuncelle(int id)
        {
            List<SelectListItem> EgitimListesi = (from x in _appDbContext.Egitims.ToList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.kategoriAdı,
                                                      Value = x.Id.ToString()
                                                  }).ToList();
            ViewBag.EgitimListesi = EgitimListesi;

            var Egitimler = _appDbContext.Videos.Where(x => x.Id == id).Select(y => new VideoViewModel()
            {
                Description = y.Description,
                Name = y.Name,
                EgitimId = y.EgitimId
            }).FirstOrDefault();

            return View(Egitimler);

        }

        [HttpPost]
        public async Task<IActionResult> DersGuncelle(VideoViewModel models)
        {
            var Egitimler = _appDbContext.Videos.Where(x => x.Id == models.Id).FirstOrDefault();
            Egitimler.Name = models.Name;
            Egitimler.Description = models.Description;
            Egitimler.EgitimId = models.EgitimId;
             _appDbContext.SaveChangesAsync();
            _notyfService.Success("Güncelleme işlemi Başarılı");
            return RedirectToAction("Dersler", "Admin");

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Yetkili");
        }
        public async Task<IActionResult> GetUserList()
        {
            var userModels = await _userManager.Users.Select(x => new RegisterModelView()
            {
                FullName = x.FullName,
                Email = x.Email,
                UserName = x.UserName,
                City = x.City
            }).ToListAsync();
            return View(userModels);
        }
        public async Task<IActionResult> GetRoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }


        [HttpGet]
        public IActionResult Iletisim()
        {
            var iletisim = _appDbContext.Iletisims.Select(x => new IletisimViewModel()
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.Name,
                Surname = x.Surname,
                Message = x.Message,

            }).ToList();
            return View(iletisim);
        }

        public IActionResult IletisimDelete(int ıd)
        {
            var iletisimdelete = _appDbContext.Iletisims.Where(x => x.Id == ıd).FirstOrDefault();
            _appDbContext.Iletisims.Remove(iletisimdelete);
            _appDbContext.SaveChanges();
            return RedirectToAction("Iletisim", "Admin");
        }




    }
}
