using UnityEngine;

namespace UnionAvatars.UI
{
    public class InfiniteRotation : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}
