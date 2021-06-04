using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Categories
{
    public record CategoryGroupWithCategories
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }

        /// <summary>
        ///     Whether or not the category group is hidden
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        ///     Whether or not the category group has been deleted. Deleted category groups will only be included in delta
        ///     requests.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        ///     Category group categories. Amounts (budgeted, activity, balance, etc.) are specific to the current budget month
        ///     (UTC).
        /// </summary>
        [JsonPropertyName("categories")]
        public List<Category> Categories { get; set; }
    }
}
