using System.Diagnostics;
using FunNews.Web.Models;
using FunNews.Web.Models.DTOs;
using FunNews.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FunNews.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = await _apiService.GetAsync<List<NewsArticleDTO>>("api/odata/NewsArticles?$filter=NewsStatus eq true");
            return View(articles);
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
