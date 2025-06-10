using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.Categories
{
    [Authorize(Roles = "Staff,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IList<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        [BindProperty(SupportsGet = true)]
        public bool? FilterIsActive { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var url = "Categories";
            if (FilterIsActive.HasValue)
            {
                url += $"?$filter=IsActive eq {FilterIsActive.Value.ToString().ToLower()}";
            }
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Categories = await response.Content.ReadFromJsonAsync<IList<CategoryDTO>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync("Cookies");
                HttpContext.Session.Clear();
                RedirectToPage("/Account/Login");
            }
            else
            {
                ModelState.AddModelError("", "Failed to load categories.");
            }
        }
    }
}
