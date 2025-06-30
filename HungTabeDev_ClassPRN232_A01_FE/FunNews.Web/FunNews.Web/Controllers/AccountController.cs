using FunNews.Web.Models.DTOs;
using FunNews.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FunNews.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập, chuyển hướng về Home
            if (HttpContext.Session.GetString("Email") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<dynamic>("api/odata/SystemAccounts/login", model);
                var token = response.GetProperty("token").GetString(); // Thay "token" bằng tên thực tế trong JSON
                var email = response.GetProperty("email").GetString();
                var roleId = response.GetProperty("roleid").GetInt32();

                // Lưu thông tin vào session
                SessionExtensions.SetString(HttpContext.Session, "JwtToken", token);
                SessionExtensions.SetString(HttpContext.Session, "Email", email);
                SessionExtensions.SetInt32(HttpContext.Session, "RoleId", roleId);

                // Lưu tên vai trò để hiển thị trong navbar
                var roleName = roleId switch
                {
                    0 => "Admin",
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "Unknown"
                };
                HttpContext.Session.SetString("RoleName", roleName);

                // Thông báo đăng nhập thành công
                TempData["Success"] = "Login successful! Welcome back.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid email or password. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _apiService.PostAsync<SystemAccountDTO>("api/odata/SystemAccounts/register", model);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }



        //[HttpGet]
        //public async Task<IActionResult> Manage()
        //{
        //    if (HttpContext.Session.GetInt32("RoleId") != 0)
        //        return Unauthorized();

        //    var token = HttpContext.Session.GetString("JwtToken");
        //    var accounts = await _apiService.GetAsync<List<SystemAccountDTO>>("api/odata/SystemAccounts", token);
        //    return View(accounts);
        //}

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("Email") == null)
                return RedirectToAction("Login");

            var token = HttpContext.Session.GetString("JwtToken");
            var email = HttpContext.Session.GetString("Email");
            var accounts = await _apiService.GetAsync<List<SystemAccountDTO>>("api/odata/SystemAccounts", token);
            var account = accounts.FirstOrDefault(a => a.AccountEmail == email);

            if (account == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(SystemAccountDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JwtToken");
            try
            {
                await _apiService.PutAsync($"api/odata/SystemAccounts/{model.AccountId}", model, token);
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            if (HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();

            var token = HttpContext.Session.GetString("JwtToken");
            var accounts = await _apiService.GetAsync<List<SystemAccountDTO>>("api/odata/SystemAccounts", token);
            return View(accounts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JwtToken");
            try
            {
                await _apiService.PostAsync<SystemAccountDTO>("api/odata/SystemAccounts/register", model, token);
                TempData["Success"] = "Account created successfully.";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(short id)
        {
            if (HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();

            var token = HttpContext.Session.GetString("JwtToken");
            var account = await _apiService.GetAsync<SystemAccountDTO>($"api/odata/SystemAccounts/{id}", token);
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(short id, SystemAccountDTO account)
        {
            if (!ModelState.IsValid)
                return View(account);

            var token = HttpContext.Session.GetString("JwtToken");
            try
            {
                await _apiService.PutAsync($"api/odata/SystemAccounts/{id}", account, token);
                TempData["Success"] = "Account updated successfully.";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(account);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            try
            {
                await _apiService.DeleteAsync($"api/odata/SystemAccounts/{id}", token);
                TempData["Success"] = "Account deleted successfully.";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Manage");
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (HttpContext.Session.GetString("Email") == null)
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JwtToken");
            var email = HttpContext.Session.GetString("Email");
            var accounts = await _apiService.GetAsync<List<SystemAccountDTO>>("api/odata/SystemAccounts", token);
            var account = accounts.FirstOrDefault(a => a.AccountEmail == email);

            if (account == null)
                return RedirectToAction("Login", "Account");

            var articles = await _apiService.GetAsync<List<NewsArticleDTO>>($"api/odata/NewsArticles/ByUser/{account.AccountId}", token);
            return View(articles);
        }

    }
}