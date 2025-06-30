using System.Text.Json.Serialization;

namespace FunNews.Web.Models.DTOs
{
    public class CategoryDTO
    {
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("categoryDesciption")]
        public string categoryDesciption { get; set; } 

        [JsonPropertyName("parentCategoryId")]
        public int ParentCategoryId { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        // Bỏ qua inverseParentCategory và newsArticles để tránh vòng lặp
        [JsonIgnore]
        [JsonPropertyName("inverseParentCategory")]
        public List<CategoryDTO> InverseParentCategory { get; set; }

        [JsonIgnore]
        [JsonPropertyName("newsArticles")]
        public List<NewsArticleDTO> NewsArticles { get; set; }
    }
}