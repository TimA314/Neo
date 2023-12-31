using System.Text.Json;

namespace Neo.Utilities
{
    public static class JsonUtil
    {
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
    }

}
