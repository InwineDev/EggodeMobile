using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gizmoPosition : MonoBehaviour
{
    [SerializeField] private Transform obj;

    [SerializeField] private strelka_pos vibron;

    private Camera cam;

    [SerializeField] private int isX;
    private Vector3 offset;
    private float distance;
    [SerializeField] private LayerMask targetLayers;
    private bool isDragging;

    public void OnEnable() 
    {
        obj = vibron.vibron.transform;

        transform.parent.position = obj.position;
    }

    void Start()
    {
        cam = Camera.main;
        distance = Vector3.Distance(cam.transform.position, obj.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
                    offset = obj.position - mouseWorldPos;
                    vibron.active = false;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            vibron.active = true;
        }
        obj = vibron.vibron.transform;
        distance = Vector3.Distance(cam.transform.position, obj.position);
        transform.localScale = new Vector3(distance, distance, distance);

        if (isDragging)
        {
            transform.parent.position = obj.position;
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Vector3 newPosition = mouseWorldPos + offset;

            if (isX == 0) obj.position = new Vector3(newPosition.x, obj.position.y, obj.position.z);
            if (isX == 1) obj.position = new Vector3(obj.position.x, newPosition.y, obj.position.z);
            if (isX == 2) obj.position = new Vector3(obj.position.x, obj.position.y, newPosition.z);
            vibron.AddPosition();
        } 
        else if(vibron.vibron == null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        transform.parent.position = obj.position;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        return cam.ScreenToWorldPoint(mousePos);
    }
}