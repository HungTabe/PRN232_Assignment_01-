using FunNews.Web.Models.ViewModels;
using FunNews.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FunNews.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiService _apiService;

        public ReportController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ReportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReportViewModel model)
        {
            if (HttpContext.Session.GetInt32("RoleId") != 0)
                return Unauthorized();

            var token = HttpContext.Session.GetString("JwtToken");
            var query = $"?startDate={model.StartDate:yyyy-MM-dd}&endDate={model.EndDate:yyyy-MM-dd}";
            var report = await _apiService.GetAsync<List<ReportItem>>($"api/odata/NewsArticles/Report{query}", token);
            model.ReportItems = report;
            return View(model);
        }
    }
}