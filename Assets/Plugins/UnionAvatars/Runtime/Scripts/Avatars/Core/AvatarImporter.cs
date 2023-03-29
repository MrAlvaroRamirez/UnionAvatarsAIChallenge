using Siccity.GLTFUtility;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnionAvatars.Avatars
{
    [Preserve] public class AvatarImporter
    {
        /// <summary>
        /// Imports an avatar file and creates a new GameObject
        /// </summary>
        /// <param name="bytes">
        /// The absolute path of the avatar .glb
        /// </param>
        /// <param name="onFinished">
        /// Callback once the import process is finished. It returns the created GameObject
        /// </param>
        public static void ImportResource(byte[] bytes, Action<GameObject> onFinished)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                onFinished.Invoke(Importer.LoadFromBytes(bytes, new ImportSettings()));
            #else
                Importer.ImportGLBAsync(bytes, new ImportSettings(), (obj, anims) => onFinished?.Invoke(obj));
            #endif
        }

        public static void ImportAvatarAsHumanoid(byte[] bytes, RuntimeAnimatorController animController, Action<GameObject, bool> onFinished)
        {
            ImportResource(bytes, (avatarObj) => {
                onFinished?.Invoke(avatarObj, avatarObj.ConvertAvatarToHumanoid(animController));
            });
        }

        //TODO: Head Import and vertex weight Assign
        public static void ImportAvatarAsHumanoid(byte[] headBytes, byte[] bodyBytes, RuntimeAnimatorController animController, Action<GameObject, bool> onFinished)
        {
            ImportResource(bodyBytes, (bodyObj) => {
                ImportResource(headBytes, (headObj) => {
                    var builtAvatar = HumanoidConverter.AttachHeadToBodyArmature(bodyObj, headObj);
                    onFinished?.Invoke(builtAvatar, builtAvatar.ConvertAvatarToHumanoid(animController));
                });
            });
        }
    }
}