using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Categories
{
    public class CategoryModel
    {
        [JsonPropertyName("category_groups")] public List<CategoryGroupWithCategories>? CategoryGroups { get; set; }

        /// <summary>
        ///     The knowledge of the server
        /// </summary>
        [JsonPropertyName("server_knowledge")]
        public long ServerKnowledge { get; set; }
    }
}
