using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebApp.Models
{
    public class LoginModelDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
