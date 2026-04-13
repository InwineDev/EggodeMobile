using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostAndLobbyKostil : MonoBehaviour
{
    public void Host()
    {
        FindObjectOfType<NetworkManager>().StartHost();
    }
}
