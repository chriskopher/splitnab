using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.ApiResponse
{
    public record ErrorsResponse
    {
        [JsonPropertyName("errors")] public List<string> Errors { get; set; }
    }
}
