using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebApp.Models
{
    public class RegisterModelDTO
    {
        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage = "Role must be 0 (Admin), 1 (Staff), or 2 (Lecturer).")]
        public short AccountRole { get; set; }
    }
}
