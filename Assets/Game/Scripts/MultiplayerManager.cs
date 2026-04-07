using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager instance;
    private NetworkManager networkManager;

    public List<string> lobbyIDs = new List<string>();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void CreateLobby(int type, int peoples)
    {
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found.");
            return;
        }

        networkManager.StartHost();
        Debug.Log("Host started without Steam.");
    }

    public void GetLobbiesList()
    {
        Debug.Log("Lobby list is disabled because Steam matchmaking was removed.");
    }
}