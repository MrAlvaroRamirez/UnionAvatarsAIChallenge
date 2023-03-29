using System.Collections.Generic;
using UnityEngine;

namespace UnionAvatars.Utils
{
    public static class Extensions
    {
        public static bool TryFindBFS(this Transform aParent, string aName, out Transform child)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                {
                    child = c;
                    return true;
                }
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            child = null;
            return false;
        } 

        public static Transform FindBFS(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                {
                    return c;
                }
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        } 

        public static void SetLayer<T>
                   (this GameObject gameobject, int layer, bool includeChildren = false)
                    where T : Component
        {
            gameobject.layer = layer;
            if (includeChildren == false) return;
 
            var arr = gameobject.GetComponentsInChildren<T>(true);
            for (int i = 0; i < arr.Length; i++)
                arr[i].gameObject.layer = layer;
        }
    }
}