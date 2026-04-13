using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDontWantBeActive : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            gameObject.SetActive(false);
        }
    }
}
