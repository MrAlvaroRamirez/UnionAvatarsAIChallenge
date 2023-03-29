using UnityEngine;
using TMPro;
using UnionAvatars.API;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Threading;

namespace UnionAvatars.UI
{
    public class CreationBaseUI : UIModule
    {
        [SerializeField] private UIModule backModule;
        [SerializeField] private UIModule firstStepModule;
        [SerializeField] private UIModule lastStepModule;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private CloseDialog closeDialog;
        [SerializeField] private Button backButton;
        public byte[] cachedAvatarHead;
        public Head cachedAvatarHeadData;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private string cachedPhoto;
        
        private void Start()
        {
            (root as BaseModule).currentClosingMode = ClosingMode.Warn;
            SwapChild(firstStepModule);
        }

        public async void CreateAvatarRequest(string base64Photo)
        {
            CloseRecursive(child);
            loadingText.text = "Processing your photo...";
            loadingUI.SetActive(true);

            //Cache the photo to be used later in the final request
            cachedPhoto = base64Photo;

            HeadRequest headRequest = new HeadRequest()
            {
                Name = Guid.NewGuid().ToString(),
                SelfieImg = base64Photo
            };

            Head avatarHead = await uiManager.session.CreateHead(headRequest);

            if(cancellationToken.Token.IsCancellationRequested)
            {
                _ = uiManager.session.DeleteHead(avatarHead);
                return;
            }

            cachedAvatarHeadData = avatarHead;

            loadingUI.SetActive(true);

            if(avatarHead != null)
            {
                cachedAvatarHead =  await ResourceDownloader.Download(avatarHead, cancellationToken.Token);
                SwapChild(lastStepModule);
            }
            else
            {
                SwapChild(firstStepModule);
            }

            loadingUI.SetActive(false);
        }

        public async void OnAvatarCreated(string avatarName, Body selectedBody)
        {
            //Launch final request
            AvatarRequest avatarRequest = new AvatarRequest()
            {
                Name = (avatarName == "") ? Guid.NewGuid().ToString() : avatarName,
                BodyId = selectedBody.Id,
                CreateThumbnail = true,
                HeadId = cachedAvatarHeadData.Id
            };

            cachedAvatarHeadData = null;

            CloseRecursive(child);
            loadingText.text = "Loading your avatar...";
            loadingUI.SetActive(true);

            var newAvatar = await uiManager.session.CreateAvatar(avatarRequest);

            cancellationToken.Token.ThrowIfCancellationRequested();

            CloseRecursive(root);
            uiManager.ReturnAvatar(newAvatar);
        }

        protected override void OnExitModule()
        {
            if(cachedAvatarHeadData != null)
                _ = uiManager.session.DeleteHead(cachedAvatarHeadData);

            (root as BaseModule).currentClosingMode = ClosingMode.Immediate;
        }
        
        public void GoToBackModule()
        {
            backButton.interactable = false;
            CloseDialog newDialog = Instantiate(closeDialog, transform);
            newDialog.SetupCloseDialog("The new avatar will be lost, are you sure?",
                                        () => {newDialog.Close(); backButton.interactable = true;},
                                        () => SwapModule(backModule));
        }     

        private void OnDestroy()
        {
            cancellationToken.Cancel();
        }
    }
}