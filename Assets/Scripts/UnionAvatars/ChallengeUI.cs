using System;
using System.Threading;
using UnionAvatars.API;
using UnionAvatars.Avatars;
using UnionAvatars.UI;
using UnityEngine;

public class ChallengeUI : MonoBehaviour
{
    //Credentials

    [Tooltip("Set this to true to automatically sign in with your credentials")]
    public bool useDevCredentials;
    public string devUsername;
    public string devPassword;

    //Avatar
    
    [Tooltip("The animator that will be assigned to the avatar on it's creation")]
    [SerializeField] private RuntimeAnimatorController avatarAnimator;

    [Tooltip("The body type that the avatar will be spawned with")]
    [SerializeField] private BodyType bodyType; 
    [SerializeField] private Vector3 avatarInitialPosition; 
    [SerializeField] private Vector3 avatarInitialRotation; 

    //UI

    [SerializeField] private bool loadUIOnStart = true;
    private bool isUILoaded = false;

    //
    
    [Tooltip("Prefab of the Union Avatars UI, don't change this unless you are working with a modified version of the UI")]
    [SerializeField] private GameObject uiPrefab;
    private ServerSession session;
    private CancellationTokenSource cancellationToken = new CancellationTokenSource();
    const string zentaiMaleURL = "https://union-body.s3.eu-central-1.amazonaws.com/v2_phr_male_LR_OnesieG6.glb";
    const string zentaiFemaleURL = "https://union-body.s3.eu-central-1.amazonaws.com/v2_phr_female_LR_OnesieG6.glb";
    const string shirtMaleURL = "https://union-body.s3.eu-central-1.amazonaws.com/v2_phr_male_LR_TurtleNeck_white.glb";
    const string shirtFemaleURL = "https://union-body.s3.eu-central-1.amazonaws.com/v2_phr_female_LR_TurtleNeck_white.glb";
    
    private async void Start()
    {
        session = new ServerSession("https://endapi.unionavatars.com/", true, cancellationToken.Token);

        if(useDevCredentials)
        {
            bool logged = await session.Login(devUsername, devPassword);
            if(logged == false)
            {
                Debug.LogError("Invalid Dev Credentials");
            }
        }

        if(loadUIOnStart) LoadUI();
    }

    /// <summary>
    /// Load the UI in the scene
    /// </summary>
    private void LoadUI()
    {
        if(isUILoaded) return;

        isUILoaded = true;

        AvatarUIManager unionUI = Instantiate(uiPrefab).GetComponent<AvatarUIManager>();

        unionUI.SetupUI(session);

        unionUI.onAvatarSelected += BuildAvatar;
        unionUI.onClose += () => isUILoaded = false;
    }

    /// <summary>
    /// Loads an avatar into scene using the selected body
    /// </summary>
    private async void BuildAvatar(AvatarMetadata avatar)
    {
        if(avatar == null)
            throw new System.ArgumentNullException("avatar");
        
        //Download the head of the avatar
        Head avatarHead = await session.GetHead(avatar.Head.ToString());
        var downloadedHead = await ResourceDownloader.Download(avatarHead, cancellationToken.Token);

        Body avatarBody = new Body
        {
            Id = Guid.NewGuid()
        };

        switch (bodyType)
        {
            case BodyType.Zentai_Male:
                avatarBody.Url = new Uri(zentaiMaleURL);
                break;
            case BodyType.Shirt_Male:
                avatarBody.Url = new Uri(shirtMaleURL);
                break;
            case BodyType.Zentai_Female:
                avatarBody.Url = new Uri(zentaiFemaleURL);
                break;
            case BodyType.Shirt_Female:
                avatarBody.Url = new Uri(shirtFemaleURL);
                break;
        }

        //Download body
        var downloadedBody = await ResourceDownloader.Download(avatarBody, cancellationToken.Token);
        
        AvatarImporter.ImportAvatarAsHumanoid(downloadedHead, downloadedBody, avatarAnimator, SetupAvatar);
    }

    private void SetupAvatar(GameObject avatarObject, bool isValid)
    {
        if(!isValid) return; 

        avatarObject.transform.position = avatarInitialPosition;
        avatarObject.transform.eulerAngles = avatarInitialRotation;
    }

    private void OnDestroy()
    {   
        cancellationToken.Cancel();
    }
}

public enum BodyType
{
    Zentai_Male, Shirt_Male, Zentai_Female, Shirt_Female
}