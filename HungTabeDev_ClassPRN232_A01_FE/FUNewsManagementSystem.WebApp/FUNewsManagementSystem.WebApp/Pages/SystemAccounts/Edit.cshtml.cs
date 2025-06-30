using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public SystemAccountDTO Input { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.GetAsync($"SystemAccounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                Input = await response.Content.ReadFromJsonAsync<SystemAccountDTO>();
                return Page();
            }
            return response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? RedirectToPage("/Account/Login")
                : NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.PutAsJsonAsync($"SystemAccounts/{Input.AccountId}", Input);
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
                ModelState.AddModelError("", $"Failed to update account: {error}");
                return Page();
            }
        }
    }
}
