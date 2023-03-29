using UnityEngine;
using UnityEditor;

namespace UnionAvatars.Examples
{
    [CustomEditor(typeof(AvatarPlayerBuilderExample))]
    public class AvatarPlayerBuilderEditor : Editor {
        SerializedProperty useLink;

        void OnEnable()
        {
            useLink = serializedObject.FindProperty("UseLink");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(useLink);

            if(useLink.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarLink"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("APIURL"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Username"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Password"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarId"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerAnimator"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AttachCamera"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}