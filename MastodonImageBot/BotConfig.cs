using Newtonsoft.Json;

namespace MastodonImageBot
{
    public struct BotConfig
    {
        [JsonProperty("instance")]
        public string Instance { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("email")]
        public string Email { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }
        [JsonProperty("tags")]
        public string Tags { get; private set; }
        [JsonProperty("interval")]
        public int PostInterval { get; private set; }
    }
}
