using UnionAvatars.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UploadFileButton : MonoBehaviour,  IPointerDownHandler
{
    public SelfieUI selfieManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        SelfieUI.UploadFile(selfieManager.gameObject.name, "OnFileUpload", ".png, .jpg, .jpeg", false);
        #endif
    }
}
