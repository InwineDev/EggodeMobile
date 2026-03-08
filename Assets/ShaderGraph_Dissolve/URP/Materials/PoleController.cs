using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PoleController : MonoBehaviour
{
    private Material material;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        material.SetFloat("_Dissolve", distance);
    }
}
