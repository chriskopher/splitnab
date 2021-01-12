using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.ApiResponse
{
    public class ErrorsResponse
    {
        [JsonPropertyName("errors")] public List<string> Errors { get; set; }
    }
}
