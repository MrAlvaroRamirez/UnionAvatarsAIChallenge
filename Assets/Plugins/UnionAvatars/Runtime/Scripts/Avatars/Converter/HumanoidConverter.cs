using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;
using UnionAvatars.Utils;
using UnityEngine.Scripting;

namespace UnionAvatars.Avatars
{
    [Preserve] public static class HumanoidConverter
    {
        //Conversion: Union Avatars Mapping -> Unity Humanid Mapping
        static Dictionary<string, string> boneMapping = new Dictionary<string, string>
        {
            //Union Avatars Bone / Unity Humanoid Bone
            {"Hips", "Hips"},
            {"LeftUpLeg", "LeftUpperLeg"},
            {"RightUpLeg", "RightUpperLeg"},
            {"LeftLeg", "LeftLowerLeg"},
            {"RightLeg", "RightLowerLeg"},
            {"LeftFoot", "LeftFoot"},
            {"RightFoot", "RightFoot"},
            {"Spine", "Spine"},
            {"Spine1", "Chest"},
            {"Neck", "Neck"},
            {"Head", "Head"},
            {"LeftShoulder", "LeftShoulder"},
            {"RightShoulder", "RightShoulder"},
            {"LeftArm", "LeftUpperArm"},
            {"RightArm", "RightUpperArm"},
            {"LeftForeArm", "LeftLowerArm"},
            {"RightForeArm", "RightLowerArm"},
            {"LeftHand", "LeftHand"},
            {"RightHand", "RightHand"},
            {"LeftToeBase", "LeftToes"},
            {"RightToeBase", "RightToes"},
            {"Spine2", "UpperChest"},
            {"LeftHandThumb1", "Left Thumb Proximal"},
            {"LeftHandThumb2", "Left Thumb Intermediate"},
            {"LeftHandThumb3", "Left Thumb Distal"},
            {"LeftHandIndex1", "Left Index Proximal"},
            {"LeftHandIndex2", "Left Index Intermediate"},
            {"LeftHandIndex3", "Left Index Distal"},
            {"LeftHandMiddle1", "Left Middle Proximal"},
            {"LeftHandMiddle2", "Left Middle Intermediate"},
            {"LeftHandMiddle3", "Left Middle Distal"},
            {"LeftHandRing1", "Left Ring Proximal"},
            {"LeftHandRing2", "Left Ring Intermediate"},
            {"LeftHandRing3", "Left Ring Distal"},
            {"LeftHandPinky1", "Left Little Proximal"},
            {"LeftHandPinky2", "Left Little Intermediate"},
            {"LeftHandPinky3", "Left Little Distal"},
            {"RightHandThumb1", "Right Thumb Proximal"},
            {"RightHandThumb2", "Right Thumb Intermediate"},
            {"RightHandThumb3", "Right Thumb Distal"},
            {"RightHandIndex1", "Right Index Proximal"},
            {"RightHandIndex2", "Right Index Intermediate"},
            {"RightHandIndex3", "Right Index Distal"},
            {"RightHandMiddle1", "Right Middle Proximal"},
            {"RightHandMiddle2", "Right Middle Intermediate"},
            {"RightHandMiddle3", "Right Middle Distal"},
            {"RightHandRing1", "Right Ring Proximal"},
            {"RightHandRing2", "Right Ring Intermediate"},
            {"RightHandRing3", "Right Ring Distal"},
            {"RightHandPinky1", "Right Little Proximal"},
            {"RightHandPinky2", "Right Little Intermediate"},
            {"RightHandPinky3", "Right Little Distal"}
        };
        
        /// <summary>
        /// Converts an armature GameObject into a unity's humanoid compatible armature
        /// </summary>
        /// <param name="avatar">
        /// The avatar's armature GameObject
        /// </param>
        /// <param name="controller">
        /// The animator controller to assign
        /// </param>
        public static bool ConvertAvatarToHumanoid(this GameObject avatar, RuntimeAnimatorController controller)
        {
            if (avatar == null)
                throw new ArgumentNullException("avatar");

            //Pose Correction
            string[] avatarInfo = AvatarPoseUtils.GetInfoFromBody(avatar);

            if(avatarInfo == null ||avatarInfo.Length < 2)
                return false;
                //throw new ArgumentException("Avatar missing gender and version information", "avatar");

            AvatarPoseUtils.EnforceTPose(avatar, avatarInfo[0], avatarInfo[1]);

            var humanDescription = new HumanDescription
            {
                skeleton = CreateSkeleton(avatar),
                human = boneMapping.Select(mapping =>
                {
                    var bone = new HumanBone {humanName = mapping.Value, boneName = mapping.Key};
                    bone.limit.useDefaultValues = true;
                    return bone;
                }).ToArray(),
            };

            //Build unity's avatar
            var humanoidAvatar = AvatarBuilder.BuildHumanAvatar(avatar, humanDescription);
            humanoidAvatar.name = avatar.name;

            if (!humanoidAvatar.isValid)
                throw new ArgumentException("Couldn't create a humanoid avatar from the selected object", "avatar");

            //Add the animator component and assing the created avatar
            var animator = avatar.AddComponent<Animator>();
            animator.applyRootMotion = true;
            animator.avatar = humanoidAvatar;
            animator.runtimeAnimatorController = controller;

            return true;
        }

        private static SkeletonBone[] CreateSkeleton(GameObject avatarRoot)
		{
			List<SkeletonBone> skeleton = new List<SkeletonBone>();

			Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
			foreach (Transform avatarTransform in avatarTransforms)
			{
				SkeletonBone bone = new SkeletonBone()
				{
					name = avatarTransform.name,
					position = avatarTransform.localPosition,
					rotation = avatarTransform.localRotation,
					scale = avatarTransform.localScale
				};

				skeleton.Add(bone);
			}
			return skeleton.ToArray();
		}

        /// <summary>
        /// Attach a head-only avatar to a body
        /// </summary>
        /// <param name="body">Body GameObject, must contain an Empty-Head</param>
        /// <param name="head">Head GameObject without weights</param>
        /// <return>GameObject of the final built avatar</return>
        public static GameObject AttachHeadToBodyArmature(GameObject body, GameObject head)
        {
            if (body == null || head == null)
                throw new ArgumentNullException();

            //Pose Correction
            string[] avatarInfo = AvatarPoseUtils.GetInfoFromBody(body);

            if(avatarInfo == null ||avatarInfo.Length < 2)
                throw new ArgumentException("Body missing gender and version information", "avatar");

            //Parent head and move to correct position
            head.transform.parent = body.transform.Find("Empty-Head");
            head.transform.localPosition = Vector3.zero;
            head.transform.localScale = Vector3.one;

            //Load the head weight dictionary
            TextAsset jsonWeights = Resources.Load<TextAsset>($"UnionAvatars/HeadWeights/{avatarInfo[0]}_{avatarInfo[1]}");
            HeadBoneWeights headBoneWeights = JsonConvert.DeserializeObject<HeadBoneWeights>(jsonWeights.text);

            var boneWeightNativeArray = new NativeArray<BoneWeight1>(headBoneWeights.boneWeights, Unity.Collections.Allocator.Temp);
            var bonesPerVertexNativeArray = new NativeArray<byte>(headBoneWeights.bonesPerVertex, Unity.Collections.Allocator.Temp);

            //Assign weights to the head mesh component
            Transform headMeshTransform = head.transform.Find("UnionAvatars_Head");

            if(headMeshTransform == null)
                throw new ArgumentException("Head is missing mesh information", "head");

            SkinnedMeshRenderer headMeshComponent = headMeshTransform.GetComponent<SkinnedMeshRenderer>();
            headMeshComponent.sharedMesh.SetBoneWeights(bonesPerVertexNativeArray, boneWeightNativeArray);

            //Dipose the native arrays
            boneWeightNativeArray.Dispose();
            bonesPerVertexNativeArray.Dispose();

            //Set Head bones and bindposes
            Transform[] bones = new Transform[3];
            Matrix4x4[] bindPoses = new Matrix4x4[3];
    
            bones[2] = body.transform.FindBFS("Head");
            bindPoses[2] = bones[2].worldToLocalMatrix * headMeshComponent.localToWorldMatrix;
            bones[1] = body.transform.FindBFS("Neck");
            bindPoses[1] = bones[1].worldToLocalMatrix * headMeshComponent.localToWorldMatrix;
            bones[0] = body.transform.FindBFS("Spine2");
            bindPoses[0] = bones[0].worldToLocalMatrix * headMeshComponent.localToWorldMatrix;

            headMeshComponent.sharedMesh.bindposes = bindPoses;
            headMeshComponent.bones = bones;
            //headMeshComponent.ResetBounds();

            //Parent hair mesh to head
            head.transform.Find("UnionAvatars_Hair").parent = bones[2];

            //TODO: Improve hierarchy and naming
            return body;
        }
    }
}