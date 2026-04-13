using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gizmoScale : MonoBehaviour
{
    [SerializeField] private Transform obj;
    [SerializeField] private strelka_pos vibron;
    private Camera cam;

    [SerializeField] private int isX;
    private Vector3 offset;
    private float distance;
    private Vector3 initialScale;
    private Vector3 initialMousePos;
    private bool isDragging;
    private Vector3 factScale;
    [SerializeField] private LayerMask targetLayers;
    public void OnEnable() 
    {
        obj = vibron.vibron.transform;
        transform.parent.rotation = obj.localRotation;
        factScale = transform.localScale;
        transform.parent.position = obj.position;
    }

    void Start()
    {
        cam = Camera.main;
        distance = Vector3.Distance(cam.transform.position, obj.position);
    }
/*    void OnMouseDown()
    {
        isDragging = true;
        distance = Vector3.Distance(cam.transform.position, obj.position);
        initialMousePos = GetMouseWorldPosition();
        initialScale = obj.localScale;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }*/
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
                    initialMousePos = GetMouseWorldPosition();
                    initialScale = obj.localScale;
                    vibron.active = false;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            vibron.active = true;
            isDragging = false;
        }
        obj = vibron.vibron.transform;
        distance = Vector3.Distance(cam.transform.position, obj.position);
        transform.localScale = new Vector3(distance, distance, distance);
        if (isDragging)
        {
            transform.parent.position = obj.position;
            transform.parent.rotation = obj.localRotation;
            Vector3 currentMousePos = GetMouseWorldPosition();
            Vector3 scaleDelta = currentMousePos - initialMousePos;

            //transform.parent.transform.localScale = factScale;
            if (isX == 0) obj.localScale = initialScale + new Vector3(scaleDelta.x, 0, 0);
            if (isX == 1) obj.localScale = initialScale + new Vector3(0, scaleDelta.y, 0);
            if (isX == 2) obj.localScale = initialScale + new Vector3(0, 0, scaleDelta.z);
            vibron.AddScale();
        }
        else if (vibron.vibron == null)
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