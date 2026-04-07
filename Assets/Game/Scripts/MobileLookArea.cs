using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [HideInInspector] public Vector2 lookDelta;

    [Header("Settings")]
    public float sensitivity = 1f;
    public bool invertY = false;

    private int activePointerId = int.MinValue;
    private Vector2 lastPosition;
    private bool isDragging;
    private bool receivedDragThisFrame;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue)
            return;

        activePointerId = eventData.pointerId;
        lastPosition = eventData.position;
        lookDelta = Vector2.zero;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || eventData.pointerId != activePointerId)
            return;

        Vector2 currentPosition = eventData.position;
        Vector2 delta = currentPosition - lastPosition;
        lastPosition = currentPosition;

        if (invertY)
            delta.y = -delta.y;

        lookDelta = delta * sensitivity;
        receivedDragThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        activePointerId = int.MinValue;
        lookDelta = Vector2.zero;
        isDragging = false;
        receivedDragThisFrame = false;
    }

    private void LateUpdate()
    {
        if (!isDragging || !receivedDragThisFrame)
            lookDelta = Vector2.zero;

        receivedDragThisFrame = false;
    }
}