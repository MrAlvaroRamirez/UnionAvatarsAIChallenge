using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnionAvatars.Log;

namespace UnionAvatars.UI
{    
    public class LogMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Title;
        [SerializeField] private TextMeshProUGUI Message;
        [SerializeField] private Image Background;
        [SerializeField] private Image Border;
        private Color32 infoColor = new Color32(255, 255, 255, 255);
        private Color32 warningColor = new Color32(255, 150, 0, 255);
        private Color32 errorColor = new Color32(255, 0, 0, 255);

        public void SetupLog(string title, string message, AvatarSDKLogType logType)
        {
            Title.text = title;
            Message.text = message;

            switch(logType)
            {
                case AvatarSDKLogType.Info:
                    infoColor.a = (byte)(Background.color.a * 255);
                    Background.color = infoColor;
                    infoColor.a = (byte)(Border.color.a * 255);
                    Border.color = infoColor;
                    break;
                case AvatarSDKLogType.Warning:
                    warningColor.a = (byte)(Background.color.a * 255);
                    Background.color = warningColor;
                    warningColor.a = (byte)(Border.color.a * 255);
                    Border.color = warningColor;
                    break;
                case AvatarSDKLogType.Error:
                    errorColor.a = (byte)(Background.color.a * 255);
                    Background.color = errorColor;
                    errorColor.a = (byte)(Border.color.a * 255);
                    Border.color = errorColor;
                    break;
            }
        }
    }
}