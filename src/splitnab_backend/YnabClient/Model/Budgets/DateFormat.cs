using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    /// <summary>
    ///     The date format setting for the budget. In some cases the format will not be available and will be specified as
    ///     null.
    /// </summary>
    public class DateFormat
    {
        [JsonPropertyName("format")] public string? Format { get; set; }
    }
}
