using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebAPI.DTOs
{
    public class RegisterModel
    {
        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        [Required]
        [MinLength(6)]
        public string AccountPassword { get; set; }

        [Required]
        public int AccountRole { get; set; } // 1 = Staff, 2 = Lecturer
    }
}
