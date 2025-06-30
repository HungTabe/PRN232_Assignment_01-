using FunNews.Web.Models.DTOs;
using FunNews.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FunNews.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiService _apiService;

        public CategoryController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("RoleId") != 1 && HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();

            var token = HttpContext.Session.GetString("JwtToken");
            var categories = await _apiService.GetAsync<List<CategoryDTO>>("api/odata/Categories", token);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var token = HttpContext.Session.GetString("JwtToken");
            await _apiService.PostAsync<CategoryDTO>("api/odata/Categories", category, token);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(short id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var category = await _apiService.GetAsync<CategoryDTO>($"api/odata/Categories/{id}", token);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(short id, CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var token = HttpContext.Session.GetString("JwtToken");
            await _apiService.PutAsync($"api/odata/Categories/{id}", category, token);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            await _apiService.DeleteAsync($"api/odata/Categories/{id}", token);
            return RedirectToAction("Index");
        }
    }
}