using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dvigat : MonoBehaviour
{
    private GameObject objectToMove;
    private float scaleFactor = 1.0f;

    private void OnMouseDrag()
    {
        if (!Input.GetKey(KeyCode.R) && !Input.GetKey(KeyCode.C))
        {
            float distance_to_screen = Camera.main.WorldToScreenPoint(objectToMove.transform.position).z;
            Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
            objectToMove.transform.position = new Vector3(pos_move.x, pos_move.y, pos_move.z);
        }
        else if (!Input.GetKey(KeyCode.C))
        {
            // Поворот объекта вокруг осей X, Y и Z
            float angleX = Input.GetAxis("Mouse X") * Time.deltaTime * 500;
            float angleY = Input.GetAxis("Mouse Y") * Time.deltaTime * 500;
            objectToMove.transform.RotateAround(objectToMove.transform.position, Vector3.forward, angleX);
            objectToMove.transform.RotateAround(objectToMove.transform.position, Vector3.right, angleY);
        }
        else
        {
            scaleFactor = Mathf.Clamp(scaleFactor + Input.GetAxis("Mouse ScrollWheel") * 2f, 0.1f, 10f);
            objectToMove.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    private void OnMouseDown()
    {
        objectToMove = gameObject;
    }

    private void OnMouseUp()
    {
        objectToMove = null;
    }

    private Camera _camera;
    private Vector3 _initialScale;

    private void Start()
    {
        _camera = Camera.main;
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // Увеличение объекта в сторону, в которую тянется курсор
            Vector3 mousePosition = Input.mousePosition;
            Vector3 objectPosition = _camera.WorldToScreenPoint(transform.position);
            Vector3 direction = (mousePosition - objectPosition).normalized;

            transform.localScale = _initialScale + direction * scaleFactor;
        }
    }
}
