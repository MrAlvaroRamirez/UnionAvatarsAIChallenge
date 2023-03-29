using UnityEngine;

namespace UnionAvatars.UI
{    
    public class UIModule : MonoBehaviour
    {
        protected UIModule root;
        protected UIModule parent;
        protected AvatarUIManager uiManager;
        protected UIModule child;
        /// <summary>
        /// Parent Transform where child modules will be spawned
        /// </summary>
        public Transform ModuleContainer;

        public void SetupModule(UIModule parent, UIModule root, AvatarUIManager uiManager)
        {
            this.parent = parent;
            this.root = root ?? this;
            this.uiManager = uiManager;
        }

        public void SwapChild(UIModule module)
        {
            //Delete any previous module
            CloseRecursive(child);

            UIModule newChildModule = Instantiate(module, ModuleContainer);

            newChildModule.SetupModule(this, root, uiManager);

            child = newChildModule;
        }

        public void SwapModule(UIModule module)
        {
            if(parent == null)
                return;

            parent.SwapChild(module);
        }

        public void SwapRoot(UIModule module)
        {
            if(parent == null)
                return;

            parent.SwapChild(module);
        }

        public void CloseRecursive(UIModule module)
        {
            if(module == null) return;

            CloseRecursive(module.child);

            module.OnExitModule();
            Destroy(module.gameObject);
        }

        protected virtual void OnExitModule(){}
    }
}
