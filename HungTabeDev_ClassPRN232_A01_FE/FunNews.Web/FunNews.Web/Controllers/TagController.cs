using FunNews.Web.Models.DTOs;
using FunNews.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FunNews.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly ApiService _apiService;

        public TagController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("RoleId") != 1 && HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();

            var token = HttpContext.Session.GetString("JwtToken");
            var tags = await _apiService.GetAsync<List<TagDTO>>("api/odata/Tags", token);
            return View(tags);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TagDTO tag)
        {
            if (!ModelState.IsValid)
                return View(tag);

            var token = HttpContext.Session.GetString("JwtToken");
            await _apiService.PostAsync<TagDTO>("api/odata/Tags", tag, token);
            TempData["Success"] = "Tag created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var tag = await _apiService.GetAsync<TagDTO>($"api/odata/Tags/{id}", token);
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TagDTO tag)
        {
            if (!ModelState.IsValid)
                return View(tag);

            var token = HttpContext.Session.GetString("JwtToken");
            await _apiService.PutAsync($"api/odata/Tags/{id}", tag, token);
            TempData["Success"] = "Tag updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            try
            {
                await _apiService.DeleteAsync($"api/odata/Tags/{id}", token);
                TempData["Success"] = "Tag deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}