using System.Text.Json.Serialization;

namespace Neo.Models
{
    public class Profile
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("about")]
        public string? About { get; set; }

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("nip05")]
        public string? Nip05 { get; set; }

        [JsonPropertyName("lud06")]
        public string? Lud06 { get; set; }

        [JsonPropertyName("lud16")]
        public string? Lud16 { get; set; }
    }
}
