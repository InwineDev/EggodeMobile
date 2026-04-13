using UnityEngine;
using UnityEngine.EventSystems;

public class cursorScript : MonoBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<Collider>().bounds.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                startPosition = transform.position;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            print("SUS3452");
        }

        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, startPosition.y, startPosition.z);
        }
    }
}