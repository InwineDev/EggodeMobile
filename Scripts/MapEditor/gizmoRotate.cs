using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gizmoRotate : MonoBehaviour
{
    [SerializeField] private Transform obj;
    [SerializeField] private strelka_pos vibron;
    private Camera cam;

    [SerializeField] private int isX;
    private Vector3 offset;
    private float distance;
    private Vector3 initialRotation;
    private Vector3 initialMousePos;
    private bool isDragging;
    private Vector3 factScale;

    [SerializeField] private LayerMask targetLayers;

    public void OnEnable()
    {
        if (vibron.vibron != null) // Добавлена проверка на null
        {
            obj = vibron.vibron.transform;
            factScale = transform.localScale;
            transform.parent.position = obj.position;
        }
    }

    void Start()
    {
        cam = Camera.main;
        if (obj != null) // Добавлена проверка на null
        {
            distance = Vector3.Distance(cam.transform.position, obj.position);
        }
    }

    void Update()
    {
        if (obj == null) return; // Защита от null reference

        // Обновление позиции и масштаба
        distance = Vector3.Distance(cam.transform.position, obj.position);
        transform.localScale = new Vector3(distance, distance, distance);
        transform.parent.position = obj.position;

        // Обработка начала перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    initialMousePos = GetMouseWorldPosition(); // Сохраняем начальную позицию мыши
                    initialRotation = obj.localEulerAngles; // Сохраняем начальный поворот
                    vibron.active = false;
                }
            }
        }

        // Обработка окончания перетаскивания
        if (Input.GetMouseButtonUp(0))
        {
            vibron.active = true;
            isDragging = false;
        }

        // Обработка самого перетаскивания
        if (isDragging)
        {
            Vector3 currentMousePos = GetMouseWorldPosition();
            Vector3 mouseDelta = currentMousePos - initialMousePos;
            float rotationAmount = (mouseDelta.x + mouseDelta.y) * 10f;

            // Применяем поворот только по выбранной оси
            Vector3 newRotation = initialRotation;
            if (isX == 0) newRotation.x += rotationAmount;
            else if (isX == 1) newRotation.y += rotationAmount;
            else if (isX == 2) newRotation.z += rotationAmount;

            obj.localEulerAngles = newRotation;
            vibron.AddRotation();
        }
        else if (vibron.vibron == null)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        return cam.ScreenToWorldPoint(mousePos);
    }
}