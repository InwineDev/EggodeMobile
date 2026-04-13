using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Vector2 lookDelta;

    private int pointerId = -1;
    private Vector2 lastPosition;
    private bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        pointerId = eventData.pointerId;
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pressed || eventData.pointerId != pointerId) return;

        Vector2 currentPosition = eventData.position;
        lookDelta = currentPosition - lastPosition;
        lastPosition = currentPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != pointerId) return;

        pressed = false;
        pointerId = -1;
        lookDelta = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!pressed)
            lookDelta = Vector2.zero;
    }
}