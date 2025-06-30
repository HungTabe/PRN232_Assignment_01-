using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.NewsArticles
{
    [Authorize(Roles = "Staff,Admin")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public NewsArticleDTO Input { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public List<TagDTO> Tags { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var catResponse = await client.GetAsync("Categories?$filter=IsActive eq true");
            var tagResponse = await client.GetAsync("Tags");
            if (catResponse.IsSuccessStatusCode && tagResponse.IsSuccessStatusCode)
            {
                Categories = await catResponse.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                Tags = await tagResponse.Content.ReadFromJsonAsync<List<TagDTO>>();
                Input = new NewsArticleDTO { TagIds = new List<int>() };
                return Page();
            }
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.PostAsJsonAsync("NewsArticles", Input);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to create article: {error}");
                await OnGetAsync(); // Reload Categories and Tags
                return Page();
            }
        }
    }
}
