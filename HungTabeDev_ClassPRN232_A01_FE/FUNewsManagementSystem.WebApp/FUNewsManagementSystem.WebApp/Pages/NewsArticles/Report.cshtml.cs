using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.NewsArticles
{
    [Authorize(Roles = "Admin")]
    public class ReportModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public ReportModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public ReportInput Input { get; set; }

        public List<NewsArticleDTO> Articles { get; set; }

        public class ReportInput
        {
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
        }

        public IActionResult OnGet()
        {
            Input = new ReportInput
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now
            };
            return Page();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var url = $"NewsArticles/Report?startDate={Input.StartDate:yyyy-MM-dd}&endDate={Input.EndDate:yyyy-MM-dd}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Articles = await response.Content.ReadFromJsonAsync<List<NewsArticleDTO>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync("Cookies");
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }
            else
            {
                Articles = new List<NewsArticleDTO>();
                ModelState.AddModelError("", "Failed to generate report.");
            }
            return Page();
        }
    }
}
