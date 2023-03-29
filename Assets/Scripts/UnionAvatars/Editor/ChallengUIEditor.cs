using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChallengeUI))]
public class ChallengUIEditor : Editor 
{
    SerializedProperty useDevCredentialsProp;
    SerializedProperty devUsernameProp;
    SerializedProperty avatarAnimatorProp;
    SerializedProperty bodyTypeProp;
    SerializedProperty loadUIOnStartProp;
    SerializedProperty avatarInitialPositionProp;
    SerializedProperty avatarInitialRotationProp;

    void OnEnable()
    {
        useDevCredentialsProp = serializedObject.FindProperty("useDevCredentials");
        devUsernameProp = serializedObject.FindProperty("devUsername");
        avatarAnimatorProp = serializedObject.FindProperty("avatarAnimator");
        loadUIOnStartProp = serializedObject.FindProperty("loadUIOnStart");
        bodyTypeProp = serializedObject.FindProperty("bodyType");
        avatarInitialPositionProp = serializedObject.FindProperty("avatarInitialPosition");
        avatarInitialRotationProp = serializedObject.FindProperty("avatarInitialRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Credentials", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(useDevCredentialsProp);

        if((target as ChallengeUI).useDevCredentials)
        {
            EditorGUILayout.PropertyField(devUsernameProp);
            (target as ChallengeUI).devPassword = EditorGUILayout.PasswordField("Dev Password", (target as ChallengeUI).devPassword);
        }

        EditorGUILayout.LabelField("Avatar", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(avatarAnimatorProp);
        EditorGUILayout.PropertyField(bodyTypeProp);
        EditorGUILayout.PropertyField(avatarInitialPositionProp);
        EditorGUILayout.PropertyField(avatarInitialRotationProp);

        EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(loadUIOnStartProp);
        
        serializedObject.ApplyModifiedProperties();
    }
}