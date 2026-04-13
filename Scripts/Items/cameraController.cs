using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : NetworkBehaviour, interactable
{
    [SerializeField] private GameObject cam;
    public void interact(FirstPersonController player)
    {
        cam.SetActive(true);
    }
}
