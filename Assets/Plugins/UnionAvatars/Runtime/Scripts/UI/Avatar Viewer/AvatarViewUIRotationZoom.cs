using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarViewUIRotationZoom : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Transform AvatarParent;
    public float RotationSpeed = 1;
    public float MaxVelocity = 2;
    public float SlowDownSpeed = 1;
    private bool isDragging;
    private float velocity;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float xRotation = Input.GetAxis("Mouse X");
        velocity = xRotation;
        AvatarParent.Rotate(Vector3.down, velocity * RotationSpeed, Space.Self);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        float xRotation = Input.GetAxis("Mouse X");

        if(Mathf.Abs(xRotation) <= .3f)
        {
            velocity = 0;
        }
        else
        {
            velocity = Mathf.Clamp(velocity, -MaxVelocity, MaxVelocity);
        }
    }

    private void Update()
    {
        if(velocity == 0) return;

        if(!isDragging)
        {
            velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * SlowDownSpeed);
            AvatarParent.Rotate(Vector3.down, velocity * RotationSpeed, Space.Self);
        }
    }
}
