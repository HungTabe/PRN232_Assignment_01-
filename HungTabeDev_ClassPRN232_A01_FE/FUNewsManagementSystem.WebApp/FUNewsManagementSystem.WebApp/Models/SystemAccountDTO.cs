namespace FUNewsManagementSystem.WebApp.Models
{
    public class SystemAccountDTO
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public short AccountRole { get; set; } // 0: Admin, 1: Staff, 2: Lecturer
    }
}
