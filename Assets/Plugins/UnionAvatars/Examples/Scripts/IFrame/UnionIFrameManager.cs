using System.Runtime.InteropServices;
using UnityEngine;
using Newtonsoft.Json;
using UnionAvatars.API;
using UnionAvatars.Avatars;
using System.Threading;

namespace UnionAvatars.Examples
{
    public class UnionIFrameManager : MonoBehaviour
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern void LoadIFrame(string targetObjectName);
        #endif

        public RuntimeAnimatorController PlayerAnimator;
        public bool LoadIFrameOnStart = true;
        public bool AttachCamera;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private CameraFollow cameraFollowComponent;

        private void Start()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            //Call a Javascript function to load and IFrame in the browser
            if(LoadIFrameOnStart)
                LoadIFrame(gameObject.name);
            #endif
            //You can also call the above function using a button, for example

            if(AttachCamera)
            {
                if(!Camera.main.gameObject.TryGetComponent<CameraFollow>(out cameraFollowComponent))
                {
                    cameraFollowComponent = Camera.main.gameObject.AddComponent<CameraFollow>();
                }
            }
        }

        //Method called from JS once the created avatar data is received
        public void ReceiveAvatarData(string message)
        {
            IFrameResponse iframeAvatar = JsonConvert.DeserializeObject<IFrameResponse>(message);
            BuildAvatar(iframeAvatar.AvatarLink);
        }

        private async void BuildAvatar(System.Uri avatarLink)
        {
            var downloadedAvatar = await ResourceDownloader.Download(avatarLink, ResourceType.Avatar, cancellationToken.Token);
            AvatarImporter.ImportAvatarAsHumanoid(downloadedAvatar, PlayerAnimator, SetupPlayerComponents);
        }

        private void SetupPlayerComponents(GameObject avatarObject, bool isValid)
        {
            if(!isValid) return; 

            avatarObject.AddComponent<PlayerMovement>();

            if(AttachCamera)
                cameraFollowComponent.SetupTarget(avatarObject.transform);
        }

        private void OnDestroy()
        {   
            cancellationToken.Cancel();
        }
    }
}
