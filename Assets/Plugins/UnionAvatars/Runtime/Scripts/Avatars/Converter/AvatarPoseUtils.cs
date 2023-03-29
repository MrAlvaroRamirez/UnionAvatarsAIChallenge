using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnionAvatars.Utils;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnionAvatars.Avatars
{
    [Preserve] public class AvatarPoseUtils
    {
        public static void EnforceTPose(GameObject avatarRoot, string version, string gender)
        {
            //TODO: Refactor into a new class with a custom constructor to allow using different dictionaries
            var jsonDict = Resources.Load<TextAsset>($"UnionAvatars/BoneOffsets/{version}_{gender}");

            if(jsonDict == null)
                throw new ArgumentException("Unsupported version/gender");

            Dictionary<string, Quaternion> offsetDict = JsonConvert.DeserializeObject<Dictionary<string, Quaternion>>(jsonDict.text, new JsonSerializerSettings()
                                                                                    { 
                                                                                        Converters = new [] {
                                                                                            new Siccity.GLTFUtility.Converters.QuaternionConverter(),
                                                                                        }
                                                                                    });

            Transform[] boneTransforms = avatarRoot.GetComponentsInChildren<Transform>();
            foreach (Transform boneTransform in boneTransforms)
            {
                Quaternion boneNewRotation;

                if(offsetDict.ContainsKey(boneTransform.name))
                    boneNewRotation = offsetDict[boneTransform.name] * boneTransform.localRotation;
                else
                    boneNewRotation = boneTransform.localRotation;

                boneTransform.localRotation = boneNewRotation;
            }
        }
        
        /// <summary>
        /// Gets the version and gender info from an avatar object
        /// </summary>
        /// <returns>
        /// String[]: Version, gender, style
        /// </returns>
        public static string[] GetInfoFromBody(GameObject avatar)
        {
            //In case the avatar is a head
            //TODO: Refactor once new head API is built
            if(!avatar.transform.TryFindBFS("UnionAvatars_Body", out Transform bodyTransform))
                return null;
                //throw new ArgumentException("Avatar doesn't have a body", "avatar");

            SkinnedMeshRenderer body = bodyTransform.GetComponent<SkinnedMeshRenderer>();

            if(body == null || body.material == null || body.material.mainTexture == null)
                throw new ArgumentException("Coudln't get avatar version and gender information", "avatar");

            string[] textureInfo = body.material.mainTexture.name.Split('_');

            if(textureInfo.Length < 3)
            {
                //This body doesn't follow the naming convention
                Debug.LogWarning("Coudln't parse the correct version and gender for this body. Loading default armature");
                return new[] {"v2", "male", null};
            }

            //Check if the avatar info contains the style
            if(textureInfo[1].Contains("male"))
            {
                return new[] {textureInfo[0], textureInfo[1], null};
            }
            else if(textureInfo[2].Contains("male"))
            {
                //In case the info also contains the style (E.g.: v2_phr_male)
                return new[] {textureInfo[0], textureInfo[2], textureInfo[1]};
            }
            else
            {
                Debug.LogWarning("Coudln't parse the correct version and gender for this body. Loading default armature");
                return new[] {"v2", "male", null};
            }
        }
    }
}