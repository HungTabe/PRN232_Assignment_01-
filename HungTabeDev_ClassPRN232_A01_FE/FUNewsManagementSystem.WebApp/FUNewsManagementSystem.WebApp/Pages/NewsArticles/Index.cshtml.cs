using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.NewsArticles
{
    [Authorize(Roles = "Staff,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<NewsArticleDTO> Articles { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool? FilterStatus { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var url = "NewsArticles";
            if (FilterStatus.HasValue)
            {
                url += $"?$filter=NewsStatus eq {FilterStatus.Value.ToString().ToLower()}";
            }
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Articles = await response.Content.ReadFromJsonAsync<List<NewsArticleDTO>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                RedirectToPage("/Account/Login");
            }
            else
            {
                Articles = new List<NewsArticleDTO>();
                ModelState.AddModelError("", "Failed to load articles.");
            }
        }
    }
}
