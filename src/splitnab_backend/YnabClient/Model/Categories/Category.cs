using System;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Categories
{
    public class Category
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }

        [JsonPropertyName("category_group_id")]
        public Guid CategoryGroupId { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        /// <summary>
        ///     Whether or not the category is hidden
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        ///     If category is hidden this is the id of the category group it originally belonged to before it was hidden.
        /// </summary>
        [JsonPropertyName("original_category_group_id")]
        public Guid? OriginalCategoryGroupId { get; set; }

        [JsonPropertyName("note")] public string Note { get; set; }

        /// <summary>
        ///     Budgeted amount in milliunits format
        /// </summary>
        [JsonPropertyName("budgeted")]
        public long Budgeted { get; set; }

        /// <summary>
        ///     Activity amount in millunits format
        /// </summary>
        [JsonPropertyName("activity")]
        public long Activity { get; set; }

        /// <summary>
        ///     Balance in milliunits format
        /// </summary>
        [JsonPropertyName("balance")]
        public long Balance { get; set; }

        /// <summary>
        ///     The type of goal, if the category has a goal (TB=’Target Category Balance’, TBD=’Target Category Balance by Date’,
        ///     MF=’Monthly Funding’, NEED=’Plan Your Spending’)
        ///     Enum: [ TB, TBD, MF, NEED, ]
        /// </summary>
        [JsonPropertyName("goal_type")]
        public string GoalType { get; set; }

        /// <summary>
        ///     The month a goal was created
        /// </summary>
        [JsonPropertyName("goal_creation_month")]
        public DateTime? GoalCreationMonth { get; set; }

        /// <summary>
        ///     The goal target amount in milliunits
        /// </summary>
        [JsonPropertyName("goal_target")]
        public long? GoalTarget { get; set; }

        /// <summary>
        ///     The target month for the goal to be completed. Only some goal types specify this date.
        /// </summary>
        [JsonPropertyName("goal_target_month")]
        public DateTime? GoalTargetMonth { get; set; }

        /// <summary>
        ///     The percentage completion of the goal
        /// </summary>
        [JsonPropertyName("goal_percentage_complete")]
        public int? GoalPercentageComplete { get; set; }

        /// <summary>
        ///     Whether or not the category has been deleted. Deleted categories will only be included in delta requests.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
