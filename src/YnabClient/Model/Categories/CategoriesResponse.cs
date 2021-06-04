using System.Text.Json.Serialization;

namespace YnabClient.Model.Categories
{
    public record CategoriesResponse
    {
        [JsonPropertyName("data")] public CategoryModel Data { get; set; }
    }
}
