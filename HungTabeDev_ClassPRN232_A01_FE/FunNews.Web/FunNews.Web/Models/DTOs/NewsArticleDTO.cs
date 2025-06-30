using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FunNews.Web.Models.DTOs
{
    public class NewsArticleDTO
    {
        [JsonPropertyName("newsArticleId")]
        public string NewsArticleId { get; set; }

        [JsonPropertyName("newsTitle")]
        public string NewsTitle { get; set; }

        [JsonPropertyName("headline")]
        public string Headline { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("newsContent")]
        public string NewsContent { get; set; }

        [JsonPropertyName("newsSource")]
        public string NewsSource { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("newsStatus")]
        public bool NewsStatus { get; set; }

        [JsonPropertyName("createdById")]
        public int CreatedById { get; set; }

        [JsonPropertyName("updatedById")]
        public int UpdatedById { get; set; }

        [JsonPropertyName("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonPropertyName("category")]
        public CategoryDTO Category { get; set; }

        [JsonPropertyName("createdBy")]
        public AccountDTO CreatedBy { get; set; }

        [JsonPropertyName("tags")]
        public List<TagDTO> Tags { get; set; }
    }
}