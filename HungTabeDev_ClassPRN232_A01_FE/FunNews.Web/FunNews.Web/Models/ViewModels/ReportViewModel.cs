using System.ComponentModel.DataAnnotations;

namespace FunNews.Web.Models.ViewModels
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        public List<ReportItem> ReportItems { get; set; } = new List<ReportItem>();
    }
    public class ReportItem
    {
        public string NewsArticleId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public string CreatedByName { get; set; }
    }
}
