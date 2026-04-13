using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyboxEffect : MonoBehaviour
{
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);
    }
}
