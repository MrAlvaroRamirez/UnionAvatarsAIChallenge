using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnionAvatars.API;

namespace UnionAvatars.UI
{
    public class AvatarSlotUI : MonoBehaviour
    {
        [SerializeField] private RawImage avatarImage;
        [SerializeField] private TextMeshProUGUI avatarName;
        public Button selectButton;
        public Button deleteButton;
        private bool thumbnail = false;

        private void Awake() 
        {
            selectButton = GetComponent<Button>();
        }

        public void SetupAvatarSlot(AvatarMetadata avatar, Texture2D image)
        {
            avatarName.text = avatar.Name;
            if(image != null)
            {
                avatarImage.texture = image;
                thumbnail = true;
            }
        }

        private void OnDestroy()
        {
            //Free memory used by Texture Asset
            if(thumbnail)
                Destroy(avatarImage.texture);
        }
    }
}