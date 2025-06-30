using System.Text.Json.Serialization;

namespace FunNews.Web.Models.DTOs
{
    public class AccountDTO
    {
        [JsonPropertyName("accountId")]
        public int AccountId { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("accountEmail")]
        public string AccountEmail { get; set; }

        [JsonPropertyName("accountRole")]
        public int AccountRole { get; set; }

        [JsonPropertyName("accountPassword")]
        public string AccountPassword { get; set; } // Lưu ý: Không nên trả về password trong API thực tế

        // Bỏ qua newsArticles để tránh vòng lặp
        [JsonIgnore]
        [JsonPropertyName("newsArticles")]
        public List<NewsArticleDTO> NewsArticles { get; set; }
    }
}