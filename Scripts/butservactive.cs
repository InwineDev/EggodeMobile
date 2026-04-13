using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class butservactive : NetworkBehaviour
{
    public GameObject button;

    private void Start()
    {
        if (!isServer)
        {
            button = gameObject;
            button.SetActive(false);
        }
    }
}
