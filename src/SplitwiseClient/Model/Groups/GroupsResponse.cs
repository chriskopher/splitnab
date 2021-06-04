using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Groups
{
    public record GroupsResponse
    {
        [JsonPropertyName("groups")] public List<Group> Groups { get; set; }
    }
}
