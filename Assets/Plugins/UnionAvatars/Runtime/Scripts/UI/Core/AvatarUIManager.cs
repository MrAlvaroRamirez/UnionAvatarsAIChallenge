using System;
using UnionAvatars.API;
using UnityEngine;

namespace UnionAvatars.UI
{   
    public class AvatarUIManager : MonoBehaviour
    {
        [SerializeField] private UIModule baseModule;
        [SerializeField] private UIModule startModule;
        [SerializeField] private LogManagerUI logManager;
        public ISession session;
        public event Action<AvatarMetadata> onAvatarSelected;
        public event Action onClose;

        public void SetupUI(ISession session)
        {
            this.session = session;

            session.LogHandler.onLog += logManager.Log;

            InstantiateUI();
        }
        
        private void InstantiateUI()
        {
            //Add the base module to the canvas
            var newBaseModule = Instantiate(baseModule, transform);
            newBaseModule.transform.SetAsFirstSibling();
            newBaseModule.SetupModule(null, null, this);

            //Show the first module
            newBaseModule.SwapChild(startModule);
        }

        public void CloseUI()
        {
            onClose?.Invoke();
            Destroy(gameObject);
        }

        public void ReturnAvatar(AvatarMetadata avatar)
        {
            onAvatarSelected?.Invoke(avatar);
            CloseUI();
        }
    }
}