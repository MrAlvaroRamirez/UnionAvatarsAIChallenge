using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace UnionAvatars.Editor
{
    public class PackageDownloader
    {
        private string packageName;
        private string tempPath;

        public PackageDownloader(string packageName, string url)
        {
            this.packageName = packageName;
            tempPath = FileUtil.GetUniqueTempPathInProject();
            DownloadPackage(url);
        }

        public async void DownloadPackage(string url)
        {
            try
            {
                WebClient client = new WebClient();

                if(EditorUtility.DisplayCancelableProgressBar(packageName, "Downloading Module " + packageName + "...", 0))
                    client.CancelAsync();

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChange);

                await client.DownloadFileTaskAsync(new Uri(url), tempPath);

                AssetDatabase.importPackageFailed += OnPackageImported;

                AssetDatabase.ImportPackage(tempPath, true);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        internal static void RemovePackage(string packageName)
        {
            FileUtil.DeleteFileOrDirectory("Assets/Plugins/UnionAvatars/Modules/" + packageName);
            FileUtil.DeleteFileOrDirectory("Assets/Plugins/UnionAvatars/Modules/" + packageName + ".meta");
            AssetDatabase.Refresh();
        }

        private void OnDownloadProgressChange(object sender, DownloadProgressChangedEventArgs eventArgs)
        {
        }

        private void OnPackageImported(string packageName, string errorMessage)
        {
            Debug.LogError("Union Module Importing Error:" + errorMessage);
        }
    }
}