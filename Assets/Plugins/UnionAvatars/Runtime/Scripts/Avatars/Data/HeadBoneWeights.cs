
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnionAvatars.Avatars
{
    [Preserve] public class HeadBoneWeights
    {
        [JsonProperty("BoneWeights")]
        public BoneWeight1[] boneWeights { get; set; }

        [JsonProperty("BonesPerVertex")]
        public byte[] bonesPerVertex { get; set; }
    }
}