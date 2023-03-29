using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace UnionAvatars.API
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AvatarRequest
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
        [JsonProperty("output_format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat? AvatarOutputFormat = OutputFormat.GLB;
        [JsonProperty("style")]
        public string Style = null;
        [JsonProperty("img")]
        public string Image = null;
        [JsonProperty("head_id")]
        public Guid? HeadId = null;
        [JsonProperty("body_id", Required = Required.Always)]
        public Guid BodyId { get; set; }
        [JsonProperty("collection_id")]
        public Guid? Collection_id = null;
        [JsonProperty("create_thumbnail")]
        public bool? CreateThumbnail = null;
    }

    public class AvatarMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("output_format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat AvatarOutputFormat { get; set; }

        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("head_id")]
        public Guid? Head { get; set; }

        [JsonProperty("avatar_link")]
        public Uri AvatarLink { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("thumbnail_url")]
        public Uri ThumbnailUrl { get; set; }
    }

    public class Body
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source_type")]
        public string SourceType { get; set; }

        [JsonProperty("thumbnail_url")]
        public Uri ThumbnailUrl { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <returns>
        /// Integrer of the version number, in case it's invalid it will return -1
        /// </returns>
        public int GetVersion()
        {
            if (!Name.StartsWith("v", true, CultureInfo.CurrentCulture))
                return -1;

            return Name[1] - '0';
        }
        
        /// <returns>
        /// String of the body gender, it can be "male" or "female"
        /// </returns>
        public string GetGender()
        {
            if(Name.Contains("female"))
                return "female";
            else
                return "male";
        }
    }

    public class Head
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("output_format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat HeadOutputFormat { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("hair")]
        public Hair Hair { get; set; }

        [JsonProperty("head_metadata")]
        public HeadMetadata HeadMetadata { get; set; }
    }

    public class HeadMetadata
    {
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class HeadRequest
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
        [JsonProperty("output_format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat? HeadOutputFormat = OutputFormat.GLB;

        [JsonProperty("style")]
        public string Style = null;

        [JsonProperty("selfie_img", Required = Required.Always)]
        public string SelfieImg { get; set; }

        [JsonProperty("hair_id")]
        public Guid? HairId = null;
    }

    public class Hair
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("hair_metadata")]
        public HairMetadata HairMetadata { get; set; }

        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class HairMetadata
    {
    }

    public class IFrameResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("avatar_link")]
        public Uri AvatarLink { get; set; }

        [JsonProperty("owner_id")]
        public Guid OwnerId { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("output_format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputFormat AvatarOutputFormat { get; set; }

        [JsonProperty("thumbnail_url")]
        public object ThumbnailUrl { get; set; }
    }

    public enum OutputFormat
    {
        GLB,
        FBX
    }
}
