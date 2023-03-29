using UnionAvatars.Utils;
using UnionAvatars.API;
using UnionAvatars.Avatars;
using UnityEngine;
using System;
using System.Threading;
using Newtonsoft.Json.Utilities;

namespace UnionAvatars.UI
{
    public class AvatarViewUI : MonoBehaviour
    {
        [SerializeField] private ISession session;
        [SerializeField] private Transform avatarParent;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private RuntimeAnimatorController defaultAnimator;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private GameObject currentAvatarObject;
        public bool loading = false;

        private void Awake()
        {
            AotHelper.EnsureList<BoneWeight1>();
        }

        public void InitAvatarView(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Loads an avatar to be displayed in the UI
        /// </summary>
        /// <param name="avatarHead">Avatar Metadata of a head-only</param>
        /// <param name="body">Body Metadata to be displayed</param>
        public async void LoadAvatarView(byte[] avatarHead, Body body, Action<bool> onLoaded = null)
        {
            if(loading) return;

            loading = true;

            loadingUI.SetActive(true);

            try
            {
                var downloadedBody = await ResourceDownloader.Download(body, cancellationToken.Token);

                if(cancellationToken.IsCancellationRequested)
                    return;

                ClearPreviousAvatar();
                AvatarImporter.ImportAvatarAsHumanoid(avatarHead, downloadedBody, defaultAnimator, (avatarGO, valid) => {
                    OnAvatarDownloaded(avatarGO, valid);
                    onLoaded?.Invoke(valid);
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                loading = false;
                loadingUI.SetActive(false);
                onLoaded?.Invoke(false);
            }
        }

        /// <summary>
        /// Loads an avatar to be displayed in the UI
        /// </summary>
        /// <param name="avatar">Avatar Metadata to be displayed</param>
        public async void LoadAvatarView(AvatarMetadata avatar, Action<bool> onLoaded = null)
        {
            if(loading) return;

            loading = true;

            loadingUI.SetActive(true);

            try
            {
                var downloadedAvatar = await ResourceDownloader.Download(avatar);

                if(cancellationToken.IsCancellationRequested)
                    return;

                ClearPreviousAvatar();              
                AvatarImporter.ImportAvatarAsHumanoid(downloadedAvatar, defaultAnimator, (avatarGO, valid) => {
                    OnAvatarDownloaded(avatarGO, valid);
                    onLoaded?.Invoke(valid);
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                loading = false;
                loadingUI.SetActive(false);
                onLoaded?.Invoke(false);
            }
        }

        private void OnAvatarDownloaded(GameObject avatarGO, bool isValid)
        {
            loading = false;
            loadingUI.SetActive(false);

            if(!isValid)
            {
                Destroy(avatarGO);
                return;
            }

            currentAvatarObject = avatarGO;
            
            avatarGO.transform.parent = avatarParent;
            avatarGO.transform.localPosition = Vector3.zero;
            avatarGO.transform.localRotation = Quaternion.identity;
            avatarGO.transform.localScale = Vector3.one;

            //Add the layer to the gameobject so the UI camera will render it
            avatarGO.SetLayer<Renderer>(LayerMask.NameToLayer("Avatar"), true);

            //Workaround to show transparency correctly in UI
            var hair = avatarGO.transform.FindBFS("UnionAvatars_Hair");
            if(hair != null)
                hair.GetComponent<Renderer>().sharedMaterial.renderQueue = 2450; //AlphaTest

        }

        private void ClearPreviousAvatar()
        {
            if(currentAvatarObject != null)
            {
                Destroy(currentAvatarObject);
                Resources.UnloadUnusedAssets();
            }
        }

        private void OnDestroy()
        {   
            cancellationToken.Cancel();
        }
    }
}

