using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnionAvatars.API;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace UnionAvatars.UI
{
    public class AvatarCreationUI : UIModule
    {
        [SerializeField] private GameObject loadingBodiesUI;
        [SerializeField] private GameObject selectionUI;
        [SerializeField] private Transform avatarGrid;
        [SerializeField] private BodySlotUI bodySlotPrefab;
        [SerializeField] private AvatarViewUI avatarView;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private Body currentSelectedBody;
        [Header("Gender")]
        [SerializeField] private Button androButton;
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;
        private string currentGender = "andro";
        private List<GameObject> maleBodySlots = new List<GameObject>();
        private List<GameObject> femaleBodySlots = new List<GameObject>();
        [Header("Load Avatar")]
        [SerializeField] private InputDialog nameDialog;
        [SerializeField] private Button loadAvatarButton;
        const int bodyChunkSize = 20;
        private bool loadingBodies = false;
        private int loadedBodiesCount = 0;
        private bool loadedAllBodies = false;
        private bool avatarViewinitialized = false;

        private void Start()
        {
            avatarView.InitAvatarView(uiManager.session);

            _ = LoadBodyChunk();
        }

        private async Task LoadBodyChunk()
        {
            if(loadingBodies || loadedAllBodies) return;

            loadingBodies = true;
            loadingBodiesUI.SetActive(true);

            Body[] bodies = await uiManager.session.GetBodies(bodyChunkSize, loadedBodiesCount);

            cancellationToken.Token.ThrowIfCancellationRequested();

            if(bodies.Length == 0)
            {
                loadedAllBodies = true;
                loadingBodies = false;
                loadingBodiesUI.SetActive(false);
                return;
            }

            if (bodies == null)
            {
                uiManager.session.LogHandler.APIWarning("No bodies found");
                (parent as CreationBaseUI).GoToBackModule();
                return;
            }

            loadedBodiesCount += bodies.Length;

            List<Task> bodyImageTasks = new List<Task>();

            for (int i = 0; i < bodies.Length; i++)
            {
                Body body = bodies[i];

                //Only display v2 bodies
                if (body.GetVersion() <= 1) continue;

                var bodySlot = Instantiate(bodySlotPrefab, avatarGrid);

                //Separate bodies by gender
                if(body.GetGender() == "female")
                    femaleBodySlots.Add(bodySlot.gameObject);
                else
                    maleBodySlots.Add(bodySlot.gameObject);

                //Create a task for downloading the thumbnail
                bodyImageTasks.Add(DownloadBodyThumbnail(body, bodySlot));

                //Setup an event action to load an avatar when the slot gets pressed
                bodySlot.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    loadAvatarButton.interactable = false;
                    avatarView.LoadAvatarView((parent as CreationBaseUI).cachedAvatarHead, body, (valid) => loadAvatarButton.interactable = valid);
                    currentSelectedBody = body;
                });

                //Load the first body automatically
                if(!avatarViewinitialized)
                {
                    avatarViewinitialized = true;
                    bodySlot.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                }
            }

            await Task.WhenAll(bodyImageTasks);
            
            loadingBodiesUI.SetActive(false);
            loadingBodies = false;

            ChangeBodyGender(currentGender);
        }

        public void OnUpdateScrollbar(float value)
        {
            //If the scrollbar gets close to the end, load another body chunk
            if(value < 0.1f && !loadingBodies && !loadedAllBodies)
                _ = LoadBodyChunk();
        }

        private async Task DownloadBodyThumbnail(Body body, BodySlotUI bodySlot)
        {
            try
            {
                byte[] thumbnail = await ResourceDownloader.Download(body.ThumbnailUrl, ResourceType.Thumbnail, cancellationToken.Token, fileId: body.Id.ToString());
                var bodyTex = new Texture2D(2, 2);
                bodyTex.LoadImage(thumbnail);
                bodySlot.SetupBodySlot(bodyTex);
            }
            catch
            {
                Debug.LogWarning("Error while downloading body thumbnail for body id: " + body.Id);
            }
        }

        public void ChangeBodyGender(string gender)
        {
            if(loadingBodies) return;

            currentGender = gender;

            foreach (var maleBody in maleBodySlots)
            {
                maleBody.SetActive(gender == "male" || gender ==  "andro");
            }
            foreach (var femaleBody in femaleBodySlots)
            {
                femaleBody.SetActive(gender == "female" || gender ==  "andro");
            }

            //Toggle the buttons
            androButton.interactable = gender != "andro";
            maleButton.interactable = gender != "male";
            femaleButton.interactable = gender != "female";
        }

        //Shows an input window to input the avatar name once the creation ends
        public void ShowNamePrompt()
        {
            loadAvatarButton.interactable = false;

            InputDialog newDialog = Instantiate(nameDialog, transform);
            newDialog.SetupInputDialog("Give your avatar a name:",
                                       () => { newDialog.Close(); loadAvatarButton.interactable = true; },
                                       (name) => { (parent as CreationBaseUI).OnAvatarCreated(name, currentSelectedBody); });
        }

        private void OnDestroy()
        {
            cancellationToken.Cancel();
        }
    }
}