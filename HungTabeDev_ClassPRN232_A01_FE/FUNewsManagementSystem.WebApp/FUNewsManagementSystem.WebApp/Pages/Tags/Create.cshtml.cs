using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.Tags
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
        public TagDTO Tag { get; set; }

        public IActionResult OnGet()
        {
            Tag = new TagDTO();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await client.PostAsJsonAsync("Tags", Tag);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Tags/Index");
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
                ModelState.AddModelError("", $"Failed to create tag: {error}");
                return Page();
            }
        }
    }
}
