using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInCamera : MonoBehaviour
{
    private Camera me;
/*    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;
    public float fov = 60f;
    // Internal Variables
    private bool isZoomed = false;*/

    void Start()
    {
        me = GetComponent<Camera>();
    }

    private void Update()
    {
        me.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * 100f;
    }
}
