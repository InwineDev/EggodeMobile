using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skinviborstart : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<skindannie>().setskin();
    }
}
