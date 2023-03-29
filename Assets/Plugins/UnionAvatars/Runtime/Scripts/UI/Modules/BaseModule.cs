using UnityEngine;
using UnityEngine.UI;

namespace UnionAvatars.UI
{ 
    public class BaseModule : UIModule
    {
        [SerializeField] private CloseDialog closeDialog;
        [SerializeField] private Button closeButton;
        public ClosingMode currentClosingMode = ClosingMode.Immediate;
        public RawImage loginBackground;
        public RawImage avatarBackground;

        private void Start()
        {
            //Workaround to get the CrossFadeAlpha working
            avatarBackground.CrossFadeAlpha(0f, 0f, true);
        }

        public void Close()
        {
            switch(currentClosingMode)
            {
                case ClosingMode.Immediate:
                    CloseRecursive(this);
                    uiManager.CloseUI();
                    break;
                case ClosingMode.Warn:
                    closeButton.interactable = false;
                    CloseDialog newDialog = Instantiate(closeDialog, transform);
                    newDialog.SetupCloseDialog("The avatar will be lost, are you sure?",
                                               () => {newDialog.Close(); closeButton.interactable = false;},
                                               () => { CloseRecursive(this); uiManager.CloseUI(); });
                    break;
            }
        }

        public void SwapBackground()
        {
            avatarBackground.CrossFadeAlpha(1, .5f, true);
        }
    }

    public enum ClosingMode
    {
        Immediate,
        Warn
    }
}