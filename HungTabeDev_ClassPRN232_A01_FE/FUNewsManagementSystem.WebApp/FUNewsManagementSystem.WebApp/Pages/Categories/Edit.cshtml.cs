using FUNewsManagementSystem.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace FUNewsManagementSystem.WebApp.Pages.Categories
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
        public CategoryDTO Category { get; set; }
        public List<SelectListItem> ParentCategories { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var catResponse = await client.GetAsync($"Categories/{id}");
            var parentResponse = await client.GetAsync("Categories?$filter=IsActive eq true");
            if (catResponse.IsSuccessStatusCode && parentResponse.IsSuccessStatusCode)
            {
                Category = await catResponse.Content.ReadFromJsonAsync<CategoryDTO>();
                var categories = await parentResponse.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                ParentCategories = categories
                    .Where(c => c.CategoryId != id) // Không cho chọn chính nó
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                return Page();
            }
            else if (catResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || parentResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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
            var response = await client.PutAsJsonAsync($"Categories/{Category.CategoryId}", Category);
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
                ModelState.AddModelError("", $"Failed to update category: {error}");
                await OnGetAsync(Category.CategoryId); // Reload ParentCategories
                return Page();
            }
        }
    }
}
