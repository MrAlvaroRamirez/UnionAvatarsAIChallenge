using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnionAvatars.Examples
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private Vector3 offset;
        public float CameraSpeed = 10;
        private Camera cameraComponent;

        private void Awake()
        {
            cameraComponent = GetComponent<Camera>();
        }

        public void SetupTarget(Transform newTarget)
        {
            target = newTarget;
            offset = transform.position - newTarget.position;
        }

        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f )
            {
                cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView - 4, 16, 60);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f )
            {
                cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView + 4, 16, 60);
            }
        }

        void FixedUpdate()
        {
            if(target == null) return;

            Vector3 newPosition = target.position + offset;

            transform.position = Vector3.Lerp(transform.position, newPosition, CameraSpeed * Time.deltaTime);
        }
    }
}