using System.Text.Json.Serialization;

namespace FunNews.Web.Models.DTOs
{
    public class TagDTO
    {
        [JsonPropertyName("tagId")]
        public int TagId { get; set; }

        [JsonPropertyName("tagName")]
        public string TagName { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        // Bỏ qua newsArticles để tránh vòng lặp
        [JsonIgnore]
        [JsonPropertyName("newsArticles")]
        public List<NewsArticleDTO> NewsArticles { get; set; }
    }
}