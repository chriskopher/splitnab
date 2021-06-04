using System;
using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public record User
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
    }
}
