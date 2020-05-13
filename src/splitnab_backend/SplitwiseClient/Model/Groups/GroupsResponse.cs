using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Groups
{
    public class GroupsResponse
    {
        [JsonPropertyName("groups")] public List<Group>? Groups { get; set; }
    }
}
