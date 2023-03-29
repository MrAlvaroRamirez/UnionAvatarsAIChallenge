using UnityEngine;

namespace UnionAvatars
{
    public class UnionAvatarsSDK_Settings : ScriptableObject
    {
        public bool useCache = false;
        [HideInInspector] public bool firstTimeLoading = true;
    }
}
