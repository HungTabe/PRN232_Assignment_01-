using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;


        public LoginModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public LoginModelDTO Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            // Error not find out how url with out odata then baseUrl not work
            var response = await client.PostAsJsonAsync("odata/SystemAccounts/login", Input);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                HttpContext.Session.SetString("Token", result.Token);
                HttpContext.Session.SetString("Email", result.Email);
                HttpContext.Session.SetInt32("Roleid", result.Roleid);
                if (result.Roleid == 0) return RedirectToPage("/SystemAccounts/Index");
                return RedirectToPage("/NewsArticles/Index");
            }
            ModelState.AddModelError("", "Invalid email or password.");
            return Page();
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public string Email { get; set; }
            public int Roleid { get; set; }
        }
    }
}
