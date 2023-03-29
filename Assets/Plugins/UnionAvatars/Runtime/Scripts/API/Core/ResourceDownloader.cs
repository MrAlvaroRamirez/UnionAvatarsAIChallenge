using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnionAvatars.API
{
    public class ResourceDownloader
    {
        /// <summary>
        /// Downloads or retrieves an avatar from cache
        /// </summary>
        /// <returns>
        /// Byte[]: Avatar data
        /// </returns>
        public static Task<byte[]> Download(AvatarMetadata avatar, CancellationToken cancellationToken = default)
        {
            if(avatar == null)
                throw new ArgumentNullException("avatar");

            return IsCacheEnabled() ?  DownloadWithCache(avatar.AvatarLink, ResourceType.Avatar, cancellationToken, fileId: avatar.Id.ToString())
                                    : DownloadToMemory(avatar.AvatarLink, ResourceType.Avatar, cancellationToken);
        }

        /// <summary>
        /// Downloads or retrieves a head from cache
        /// </summary>
        /// <returns>
        /// Byte[]: Head data
        /// </returns>
        public static Task<byte[]> Download(Head head, CancellationToken cancellationToken = default)
        {
            if(head == null)
                throw new ArgumentNullException("head");

            return IsCacheEnabled() ?  DownloadWithCache(head.Url, ResourceType.Avatar, cancellationToken, fileId: head.Id.ToString())
                                    : DownloadToMemory(head.Url, ResourceType.Avatar, cancellationToken);
        }

        /// <summary>
        /// Downloads or retrieves a body from cache using the body url
        /// </summary>
        /// <returns>
        /// Byte[]: Body data
        /// </returns>
        public static Task<byte[]> Download(Body body, CancellationToken cancellationToken = default)
        {
            if(body == null)
                throw new ArgumentNullException("body");

            if(body.Url == null)
                throw new ArgumentException("This body doesn't contain a valid URL");

            return IsCacheEnabled() ? DownloadWithCache(body.Url, ResourceType.Body, cancellationToken, fileId: body.Id.ToString())
                                    : DownloadToMemory(body.Url, ResourceType.Body, cancellationToken);
        }

        public static Task<byte[]> Download(Uri resourceLink, ResourceType resourceType, CancellationToken cancellationToken = default, Action<byte[]> onCompleted = null, string fileId = null)
        {
            if(IsCacheEnabled() && resourceType == ResourceType.Avatar)
            {
                Debug.LogWarning("Caching an avatar using a link it's not supported. This avatar won't be cached. If you want to use this feature you most provide an Avatar Metadata");
                return DownloadToMemory(resourceLink, resourceType, cancellationToken, onCompleted);
            }

            return IsCacheEnabled() ? DownloadWithCache(resourceLink, resourceType, cancellationToken, onCompleted, fileId)
                                    : DownloadToMemory(resourceLink, resourceType, cancellationToken, onCompleted);
        }

        /// <summary>
        /// Downloads or retrieves a resource directly to memory
        /// </summary>
        /// <returns>
        /// Byte[]: Resource data
        /// </returns>
        public static async Task<byte[]> DownloadToMemory(Uri resourceLink, ResourceType resourceType, CancellationToken cancellationToken = default, Action<byte[]> onCompleted = null)
        {
            if (resourceLink == null)
                throw new APIOperationFailed("No Resource Link provided");

            using UnityWebRequest resourceWebRequest = UnityWebRequest.Get(resourceLink);

            var byteDownloadHandler = new DownloadHandlerBuffer();

            resourceWebRequest.downloadHandler = byteDownloadHandler;

            resourceWebRequest.SendWebRequest();

            while (!resourceWebRequest.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (resourceWebRequest.result is UnityWebRequest.Result.Success)
            {
                onCompleted?.Invoke(byteDownloadHandler.data);
                return byteDownloadHandler.data;
            }
            else
            {
                throw new APIOperationFailed(resourceWebRequest.error + ", url: " + resourceWebRequest.url);
            }
        }

        /// <summary>
        /// Downloads or retrieves an avatar from cache
        /// </summary>
        /// <returns>
        /// Byte[]: Resource data
        /// </returns>
        public static async Task<byte[]> DownloadWithCache(Uri resourceLink, ResourceType resourceType, CancellationToken cancellationToken = default, Action<byte[]> onCompleted = null, string fileId = null)
        {
            if (resourceLink == null)
                throw new APIOperationFailed("No Resource Link provided");
            
            //Store the avatar resource file name
            string resourceFileIdentifier = Path.GetFileNameWithoutExtension(resourceLink.LocalPath);

            string cachePath = "/cached_";
            string fileExtension = ".";

            switch (resourceType)
            {
                case ResourceType.Avatar:
                    cachePath += "avatars/";
                    fileExtension += "glb";
                    break;
                case ResourceType.Body:
                    cachePath += "bodies/";
                    fileExtension += "glb";
                    break;
                case ResourceType.Thumbnail:
                    cachePath += "thumbnails/";
                    fileExtension += "png";
                    break;
            }

            var localFilePath = Application.temporaryCachePath + cachePath + (fileId ?? resourceFileIdentifier) + fileExtension;

            //Check if the avatar exists in cache before downloading it
            if (File.Exists(localFilePath))
            {
                #if UNITY_WEBGL && !UNITY_EDITOR || !NET_STANDARD_2_1
                    var bytes = File.ReadAllBytes(localFilePath);
                #else
                    var bytes = await File.ReadAllBytesAsync(localFilePath);
                #endif
                onCompleted?.Invoke(bytes);
                return bytes;
            }

            using UnityWebRequest resourceWebRequest = UnityWebRequest.Get(resourceLink);

            var fileDownloadHandler = new DownloadHandlerFile(localFilePath);

            //We get the file deleted if the download is aborted
            fileDownloadHandler.removeFileOnAbort = true;
            resourceWebRequest.downloadHandler = fileDownloadHandler;

            resourceWebRequest.SendWebRequest();

            while (!resourceWebRequest.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (resourceWebRequest.result is UnityWebRequest.Result.Success)
            {
                #if UNITY_WEBGL && !UNITY_EDITOR || !NET_STANDARD_2_1
                    var bytes = File.ReadAllBytes(localFilePath);
                #else
                    var bytes = await File.ReadAllBytesAsync(localFilePath);
                #endif
                onCompleted?.Invoke(bytes);
                return bytes;
            }
            else
            {
                throw new APIOperationFailed(resourceWebRequest.error + ", url: " + resourceWebRequest.url);
            }
        }

        private static bool IsCacheEnabled()
        {
            var sdkSettings = Resources.Load<UnionAvatarsSDK_Settings>("UnionAvatars/UnionAvatarsSDK_Settings");

            return sdkSettings != null && sdkSettings.useCache == true;
        }
    }

    public enum ResourceType
    {
        Avatar,
        Body,
        Thumbnail
    }
}