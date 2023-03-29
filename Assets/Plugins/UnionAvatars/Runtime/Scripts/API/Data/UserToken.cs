using Newtonsoft.Json;
using UnityEngine.Networking;

namespace UnionAvatars.API
{
    public class UserToken
    {
        [JsonProperty("token_type")]
        public string TokenType;
        [JsonProperty("access_token")]
        public string AccessToken;
    }
}