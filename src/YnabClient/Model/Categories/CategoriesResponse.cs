using System.Text.Json.Serialization;

namespace YnabClient.Model.Categories
{
    public class CategoriesResponse
    {
        [JsonPropertyName("data")] public CategoryModel Data { get; set; }
    }
}
