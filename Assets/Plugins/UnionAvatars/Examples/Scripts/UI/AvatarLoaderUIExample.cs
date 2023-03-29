using System.Threading;
using UnionAvatars.API;
using UnionAvatars.Avatars;
using UnionAvatars.UI;
using UnityEngine;

namespace UnionAvatars.Examples
{
    public class AvatarLoaderUIExample : MonoBehaviour
    {
        private ServerSession session;
        public GameObject uiPrefab;
        public RuntimeAnimatorController PlayerAnimator;
        public KeyCode OpenInterfaceKey;
        public bool LoadUIOnStart;
        public bool AttachCamera;
        private bool isUILoaded = false;
        private GameObject playerGameObject;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private CameraFollow cameraFollowComponent;

        private void Start()
        {
            session = new ServerSession("https://endapi.unionavatars.com/", true, cancellationToken.Token);
            if(LoadUIOnStart) LoadUI();

            if(AttachCamera)
            {
                if(!Camera.main.gameObject.TryGetComponent<CameraFollow>(out cameraFollowComponent))
                {
                    cameraFollowComponent = Camera.main.gameObject.AddComponent<CameraFollow>();
                }
            }
        }

        private void LoadUI()
        {
            if(isUILoaded) return;

            isUILoaded = true;
            AvatarUIManager unionUI = Instantiate(uiPrefab).GetComponent<AvatarUIManager>();
            unionUI.SetupUI(session);
            unionUI.onAvatarSelected += BuildAvatar;
            unionUI.onClose += () => isUILoaded = false;
        }

        private async void BuildAvatar(AvatarMetadata avatar)
        {
            if(avatar == null)
                throw new System.ArgumentNullException("avatar");
                
            var downloadedAvatar = await ResourceDownloader.Download(avatar, cancellationToken.Token);
            AvatarImporter.ImportAvatarAsHumanoid(downloadedAvatar, PlayerAnimator, SetupPlayerComponents);
        }

        private void SetupPlayerComponents(GameObject avatarObject, bool isValid)
        {
            if(!isValid) return; 

            if(playerGameObject != null)
            {
                avatarObject.transform.position = playerGameObject.transform.position;
                avatarObject.transform.rotation = playerGameObject.transform.rotation;
                Destroy(playerGameObject);
            }
            
            if(AttachCamera)
                cameraFollowComponent.SetupTarget(avatarObject.transform);
                
            playerGameObject = avatarObject;

            avatarObject.AddComponent<PlayerMovement>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(OpenInterfaceKey))
            {
                LoadUI();
            }
        }

        private void OnDestroy()
        {   
            cancellationToken.Cancel();
        }
    }
}