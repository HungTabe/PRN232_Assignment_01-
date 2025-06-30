using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<SystemAccountDTO> Accounts { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterRole { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var url = "SystemAccounts";
            if (FilterRole.HasValue)
            {
                url += $"?$filter=AccountRole eq {FilterRole}";
            }
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Accounts = await response.Content.ReadFromJsonAsync<List<SystemAccountDTO>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                RedirectToPage("/Account/Login");
            }
            else
            {
                Accounts = new List<SystemAccountDTO>();
                ModelState.AddModelError("", "Failed to load accounts.");
            }
        }
    }
}
