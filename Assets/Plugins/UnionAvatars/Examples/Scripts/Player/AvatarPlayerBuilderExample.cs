using System;
using System.Threading;
using UnionAvatars.API;
using UnionAvatars.Avatars;
using UnityEngine;
//using UnionAvatars.Modules.LipSync;

namespace UnionAvatars.Examples
{
    public class AvatarPlayerBuilderExample : MonoBehaviour
    {
        public bool UseLink = false;
        [TextArea]
        public string AvatarLink = "INSERT LINK HERE";
        public string APIURL = "https://api.unionavatars.com/";
        public string Username = "YOUR USERNAME HERE";
        public string Password = "YOUR PASSWORD HERE";
        public string AvatarId = "AVATAR ID HERE";
        public RuntimeAnimatorController PlayerAnimator;
        public bool AttachCamera;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private CameraFollow cameraFollowComponent;

        private void Start()
        {
            if(UseLink)
            {
                BuildAvatarFromLink();
            }
            else
            {
                BuildAvatarFromAPI();
            }  

            if(AttachCamera)
            {
                if(!Camera.main.gameObject.TryGetComponent<CameraFollow>(out cameraFollowComponent))
                {
                    cameraFollowComponent = Camera.main.gameObject.AddComponent<CameraFollow>();
                }
            }
        }

        private async void BuildAvatarFromLink()
        {
            //We download the avatar file from the link provided
            //This function will return us the path of the downloaded file
            //The avatar is stored in a temporary cache
            Debug.Log("Downloading avatar...");
            byte[] avatarBytes = await ResourceDownloader.Download(new Uri(AvatarLink), ResourceType.Avatar, cancellationTokenSource.Token);

            //We import the avatar file into Unity asynchronously
            //Once it's finished the method "SetupAvatar" will be called
            AvatarImporter.ImportAvatarAsHumanoid(avatarBytes, PlayerAnimator, SetupPlayerComponents);
        }

        //Public overload to use with a button
        public async void BuildAvatarFromLink(string link)
        {
            Debug.Log("Downloading avatar...");
            byte[] avatarBytes = await ResourceDownloader.Download(new Uri(link), ResourceType.Avatar, cancellationTokenSource.Token);
            AvatarImporter.ImportAvatarAsHumanoid(avatarBytes, PlayerAnimator, SetupPlayerComponents);
        }

        private async void BuildAvatarFromAPI()
        {
            //Initialize a Union Avatars session
            //We will use this object as our main interface to perform operations
            //Ex: Login, Downloading Avatars,...
            ServerSession session = new ServerSession(APIURL, true, cancellationTokenSource.Token);
            
            //First we need to login
            //The ServerSession object we created will take care of keeping a record of our token access for future operations
            var logged = await session.Login(Username, Password);

            //In case the login fails
            if(!logged) return;

            //We retrieve the last avatar from the database
            //You can change this line if you want to retrieve the avatar with a different method
            //Ex: GetAvatarByID
            var avatar = await session.GetAvatar(AvatarId);

            //Now we download the avatar we just got
            //This function will return us the path of the downloaded file
            //The avatar is stored in a temporary cache
            Debug.Log("Downloading avatar...");
            byte[] avatarBytes = await ResourceDownloader.Download(avatar, cancellationTokenSource.Token);
            AvatarImporter.ImportAvatarAsHumanoid(avatarBytes, PlayerAnimator, SetupPlayerComponents);
        }

        private void SetupPlayerComponents(GameObject avatarObject, bool isValid)
        {
            if(!isValid) return; 
            
            Debug.Log("Avatar Loaded!");

            avatarObject.AddComponent<PlayerMovement>();

            if(AttachCamera)
                cameraFollowComponent.SetupTarget(avatarObject.transform);

            //OVR Lip Sync
            //Enable the module in the project setup window
            //Toolbar -> Union Avatars -> Project Setup -> Modules

            //AvatarLipSync.AddMicLipSync(avatarObject);
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}