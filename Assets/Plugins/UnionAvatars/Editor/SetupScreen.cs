using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnionAvatars.Editor
{
    [InitializeOnLoad]
    public class SetupScreen : EditorWindow
    {
        private int currentTab = 0;
        private Texture2D bannerTexture;
        private static UnionAvatarsSDK_Settings sdkSettings;

        static SetupScreen()
        {
            EditorApplication.update -= LoadSetupScreen;
            EditorApplication.update += LoadSetupScreen;
        }

        private static void LoadSetupScreen()
        {
            EditorApplication.update -= LoadSetupScreen;

            if (EditorApplication.isPlaying)
                return;

            LoadSettings();

            if(sdkSettings.firstTimeLoading == true)
            {
                ShowWindow(); 
                sdkSettings.firstTimeLoading = false;
                EditorUtility.SetDirty(sdkSettings);
            }
        }

        private static void LoadSettings()
        {
            sdkSettings = Resources.Load<UnionAvatarsSDK_Settings>("UnionAvatars/UnionAvatarsSDK_Settings"); 

            if(sdkSettings == null)
            {
                sdkSettings = ScriptableObject.CreateInstance<UnionAvatarsSDK_Settings>();

                AssetDatabase.CreateAsset(sdkSettings, "Assets/Plugins/UnionAvatars/Resources/UnionAvatars/UnionAvatarsSDK_Settings.asset");
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("Union Avatars/Project Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<SetupScreen>();
            window.maxSize = new Vector2(360f, 540f);
            window.minSize = window.maxSize;
            window.titleContent = new GUIContent("Union Avatars"); 

            LoadSettings();
        }

        private void OnGUI()
        {
            if(bannerTexture == null)
                FindBannerTexture();
                
            currentTab = GUILayout.Toolbar(currentTab, new string[] {"Welcome", "Settings", "Modules"});
            GUILayout.Label(bannerTexture, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.currentViewWidth / 3.369f));

            switch(currentTab)
            {
                case 0:
                    DrawWelcomePanel();
                    break;
                case 1:
                    DrawSettingsPanel();
                    break;
                case 2:
                    DrawModulesPanel();
                    break;
            }
        }

        private void DrawWelcomePanel()
        {
            EditorGUILayout.Space(10);

            GUIStyle titleStyle = new GUIStyle ();
            titleStyle.richText = true;
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = Color.white;

            EditorGUILayout.LabelField("<b><i>Welcome to the Union Avatars SDK!</i></b>", titleStyle);
            
            //DOCS

            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("<size=13>Start by reading the documentation:</size>", titleStyle);

            EditorGUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Public SDK Documentation", GUILayout.MaxWidth(250), GUILayout.MinHeight(40)))
            {
                Application.OpenURL("https://unionavatars.notion.site/Union-Avatars-Unity-SDK-53956c64d241482684414a196a88da3f");
            };
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //SETTINGS

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("<size=13>Change the settings:</size>", titleStyle);

            EditorGUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Project Settings", GUILayout.MaxWidth(250), GUILayout.MinHeight(40)))
            {
                currentTab = 1;
            };
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //MODULES

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("<size=13>Import custom modules:</size>", titleStyle);

            EditorGUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("SDK Modules", GUILayout.MaxWidth(250), GUILayout.MinHeight(40)))
            {
                currentTab = 2;
            };
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Support

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("<size=13>Join our Discord!</size>", titleStyle);

            EditorGUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Union Avatars Discord", GUILayout.MaxWidth(160), GUILayout.MinHeight(25)))
            {
                Application.OpenURL("https://discord.gg/q8jBvXJ3q4");
            };
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("<size=11>Contact support:</size>", titleStyle);
            EditorGUILayout.SelectableLabel("<size=11>techsupport@linkingrealities.com</size>", titleStyle);
        }

        private void DrawModulesPanel()
        {
            EditorGUILayout.Space(10);

            GUIStyle titleStyle = new GUIStyle ();
            titleStyle.richText = true;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 13;
            titleStyle.normal.textColor = Color.white;
            
            #region OvrLipSync
            EditorGUILayout.LabelField("<b>Oculus Lip Sync:</b>", titleStyle);
            if(ModuleExists("OculusLipSync"))
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.HelpBox("Oculus Lip Sync Support is installed in the project", MessageType.Info);
                GUI.backgroundColor = Color.white;
                if(GUILayout.Button("Remove Oculus Lip Sync Support", GUILayout.MinHeight(40)))
                {
                    PackageDownloader.RemovePackage("OculusLipSync");
                };
            }
            else
            {
                EditorGUILayout.HelpBox("OVR Lip Sync allows to easily integrate audio and microphone lip sync to your avatars", MessageType.Info);
                EditorGUILayout.HelpBox("This package requires the official Lip Sync solution provided by Meta at: https://developer.oculus.com/documentation/unity/audio-ovrlipsync-unity/", MessageType.Warning);
                if(GUILayout.Button("Download Oculus Lip Sync Support", GUILayout.MinHeight(40)))
                {
                    new PackageDownloader("OculusLipSync", "https://unity-external-module.s3.eu-central-1.amazonaws.com/OculusLipSync.unitypackage");
                };
            }
            EditorGUILayout.Space(10);
            #endregion
            #region Kinetix
            EditorGUILayout.LabelField("<b>Kinetix Emotes:</b>", titleStyle);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.HelpBox("Kinetix Emote SDK provides a customizable emote wheel UI you can use in your avatars", MessageType.Info);
            GUI.backgroundColor = Color.white;
            if(GUILayout.Button("Learn about Kinetix Emote SDK", GUILayout.MinHeight(40)))
            {
                Application.OpenURL("https://unionavatars.notion.site/Union-Avatars-Unity-SDK-53956c64d241482684414a196a88da3f#43a072c12dd5452a951c15fc34e956b3");
            };
            #endregion
        }

        private void DrawSettingsPanel()
        {
            EditorGUILayout.Space(10);
            
            sdkSettings.useCache = EditorGUILayout.Toggle(new GUIContent("Enable cache:", "Enables caches for avatars, bodies and thumbnails, making loading speed increase"), sdkSettings.useCache);

            EditorUtility.SetDirty(sdkSettings);
        }

        private bool ModuleExists(string moduleName)
        {
            return Directory.Exists(Application.dataPath + $"/Plugins/UnionAvatars/Modules/{moduleName}");
        }

        private void FindBannerTexture()
        {
            bannerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/UnionAvatars/Editor/Setup_Banner.png");
        }
    }
}
