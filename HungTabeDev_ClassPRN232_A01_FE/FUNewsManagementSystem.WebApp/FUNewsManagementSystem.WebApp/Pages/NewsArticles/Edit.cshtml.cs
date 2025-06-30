using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.NewsArticles
{
    [Authorize(Roles = "Staff,Admin")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public NewsArticleDTO Article { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public List<TagDTO> Tags { get; set; }
        public List<SystemAccountDTO> CreatedByAccounts { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var articleResponse = await client.GetAsync($"NewsArticles/{id}");
            var catResponse = await client.GetAsync("Categories?$filter=IsActive eq true");
            var tagResponse = await client.GetAsync("Tags");
            var accountResponse = await client.GetAsync("SystemAccounts");
            if (articleResponse.IsSuccessStatusCode && catResponse.IsSuccessStatusCode && tagResponse.IsSuccessStatusCode && accountResponse.IsSuccessStatusCode)
            {
                Article = await articleResponse.Content.ReadFromJsonAsync<NewsArticleDTO>();
                Categories = await catResponse.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                Tags = await tagResponse.Content.ReadFromJsonAsync<List<TagDTO>>();
                CreatedByAccounts = await accountResponse.Content.ReadFromJsonAsync<List<SystemAccountDTO>>();
                return Page();
            }
            else if (articleResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || catResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || tagResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || accountResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync("Cookies");
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.PutAsJsonAsync($"NewsArticles/{Article.NewsArticleId}", Article);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync("Cookies");
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to update article: {error}");
                await OnGetAsync(Article.NewsArticleId); // Reload data
                return Page();
            }
        }
    }
}
