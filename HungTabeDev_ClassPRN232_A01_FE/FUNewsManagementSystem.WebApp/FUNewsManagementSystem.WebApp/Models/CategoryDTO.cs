namespace FUNewsManagementSystem.WebApp.Models
{
    public class CategoryDTO
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
