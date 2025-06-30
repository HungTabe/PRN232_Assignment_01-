using System.ComponentModel.DataAnnotations;

namespace FunNews.Web.Models.DTOs
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Account name is required.")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string AccountPassword { get; set; }

        [Required(ErrorMessage = "Account role is required.")]
        public int AccountRole { get; set; } // 0 = Admin, 1 = Staff, 2 = Lecturer
    }
}
