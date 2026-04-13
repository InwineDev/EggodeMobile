using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc1 : MonoBehaviour
{
    private void Update()
    {
        float x = Random.Range(0f, 100f);
        float y = Random.Range(0f, 100f);
        float z = Random.Range(0f, 100f);
        Vector3 pos = new Vector3(x, y, z);
    }
}
