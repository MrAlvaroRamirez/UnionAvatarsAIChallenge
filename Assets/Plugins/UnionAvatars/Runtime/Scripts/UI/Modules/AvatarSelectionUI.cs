using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnionAvatars.API;
using UnionAvatars.Log;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnionAvatars.UI
{
    public class AvatarSelectionUI : UIModule
    {
        [SerializeField] private Transform avatarGrid;
        [SerializeField] private UIModule avatarCreationModule;
        [SerializeField] private AvatarViewUI avatarView;
        [SerializeField] private Button loadButton;
        [Header("Avatar Slot")]
        [SerializeField] private AvatarSlotUI avatarSlotPrefab;
        [SerializeField] private DeleteDialog deleteDialog;
        [Header("Load More")]
        [SerializeField] private Button loadMoreButton;
        [SerializeField] private GameObject loadingSpinner;
        [SerializeField] private TextMeshProUGUI loadMoreText;
        private AvatarMetadata selectedAvatar;
        private int loadedAvatarsCount = 0;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        const int chunkSize = 8;

        private async void Start()
        {
            avatarView.InitAvatarView(uiManager.session);
            LoadChunk();

            //Automatically load last avatar
            var lastAvatar = await uiManager.session.GetAvatars(1,0);

            if(lastAvatar.Length > 0)
            {

                avatarView.LoadAvatarView(lastAvatar[0], (valid) =>
                                                        {
                                                            loadButton.interactable = valid;
                                                            selectedAvatar = lastAvatar[0];
                                                        });
            }
        }

        public async void LoadChunk()
        {
            loadMoreText.text = "Loading Avatars...";
            loadingSpinner.SetActive(true);
            loadMoreButton.interactable = false;

            AvatarMetadata[] avatars = await uiManager.session.GetAvatars(chunkSize,loadedAvatarsCount);

            if(cancellationToken.IsCancellationRequested) return;

            if (avatars == null)
            {
                uiManager.session.LogHandler.CustomLog("No avatars", "Error while getting your avatars from the server", AvatarSDKLogType.Error);
                return;
            }
            if (avatars.Length <= 0)
            {
                if(loadedAvatarsCount > 0)
                {
                    uiManager.session.LogHandler.CustomLog("No more avatars", "All the avatars have been loaded", AvatarSDKLogType.Info);
                    loadMoreText.text = "Load More...";
                    loadingSpinner.SetActive(false);
                    loadMoreButton.interactable = true;
                }
                else
                {
                    uiManager.session.LogHandler.CustomLog("No avatars", "You don't have any avatar yet, create one!", AvatarSDKLogType.Info);
                    StartAvatarCreation();
                }
                return;
            }

            loadedAvatarsCount += avatars.Length;

            List<Task> avatarImageTasks = new List<Task>();

            for (int i = 0; i < avatars.Length; i++)
            {
                AvatarMetadata avatar = avatars[i];
                //Only display GLB avatars
                if (avatar.AvatarOutputFormat != OutputFormat.GLB)
                    continue;

                var avatarSlot = Instantiate(avatarSlotPrefab, avatarGrid);
                avatarSlot.transform.SetSiblingIndex(avatarGrid.childCount-2);

                if (avatar.ThumbnailUrl != null)
                {
                    avatarImageTasks.Add(ResourceDownloader.Download(avatar.ThumbnailUrl, ResourceType.Thumbnail, cancellationToken.Token, (thumbnail) =>
                    {
                        var avatarTex = new Texture2D(2, 2);
                        avatarTex.LoadImage(thumbnail);
                        avatarSlot.SetupAvatarSlot(avatar, avatarTex);
                    }));
                }
                else
                {
                    avatarSlot.SetupAvatarSlot(avatar, null);
                }

                //Setup an event action to load an avatar when the slot gets pressed
                avatarSlot.selectButton.onClick.AddListener(() =>
                                                            {
                                                                loadButton.interactable = false;
                                                                avatarView.LoadAvatarView(avatar, (valid) =>
                                                                {
                                                                    loadButton.interactable = valid;
                                                                    selectedAvatar = avatar;
                                                                });
                                                            });

                //Setup an event action to load an avatar when the slot gets pressed
                avatarSlot.deleteButton.onClick.AddListener(() =>
                                                            {
                                                                avatarSlot.deleteButton.interactable = false;
                                                                DeleteDialog newDialog = Instantiate(deleteDialog, transform);
                                                                newDialog.SetupDeleteDialog(avatar.Name,
                                                                                        () => { newDialog.Close(); avatarSlot.deleteButton.interactable = false;},
                                                                                        () => { newDialog.Close(); DeleteAvatar(avatar, avatarSlot); });
                                                            });
            }

            await Task.WhenAll(avatarImageTasks);

            loadMoreText.text = "Load More...";
            loadingSpinner.SetActive(false);
            loadMoreButton.interactable = true;
        }

        private void DeleteAvatar(AvatarMetadata avatar, AvatarSlotUI avatarSlot)
        {
            _ = uiManager.session.DeleteAvatar(avatar);
            Destroy(avatarSlot.gameObject);
        }

        public void SpawnAvatar()
        {
            CloseRecursive(root);
            uiManager.ReturnAvatar(selectedAvatar);
        }

        public void StartAvatarCreation()
        {
            SwapModule(avatarCreationModule);
        }

        private void OnDestroy()
        {
            cancellationToken.Cancel();
        }
    }
}