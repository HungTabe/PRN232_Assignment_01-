using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.Tags
{
    [Authorize(Roles = "Staff,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IList<TagDTO> Tags { get; set; } = new List<TagDTO>();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.GetAsync("Tags");
            if (response.IsSuccessStatusCode)
            {
                Tags = await response.Content.ReadFromJsonAsync<IList<TagDTO>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync("Cookies");
                HttpContext.Session.Clear();
                RedirectToPage("/Account/Login");
            }
            else
            {
                ModelState.AddModelError("", "Failed to load tags.");
            }
        }
    }
}
