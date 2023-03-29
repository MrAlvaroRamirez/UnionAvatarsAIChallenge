using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnionAvatars.UI
{
    public class BodySlotUI : MonoBehaviour
    {
        [SerializeField] private RawImage bodyImage;

        public void SetupBodySlot(Texture2D image)
        {
            bodyImage.texture = image;
        }

        private void OnDestroy()
        {
            //Free memory used by Texture Asset
            Destroy(bodyImage.texture);
        }
    }
}