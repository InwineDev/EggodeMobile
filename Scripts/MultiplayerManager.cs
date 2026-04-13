using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager instance;
    private NetworkManager networkManager;

    public List<string> lobbyIDs = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void CreateLobby(int peoples)
    {
        if (networkManager == null)
        {
            networkManager = FindObjectOfType<NetworkManager>();
        }

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found.");
            return;
        }

        if (NetworkServer.active || NetworkClient.isConnected)
        {
            Debug.LogWarning("Network is already running.");
            return;
        }

        networkManager.maxConnections = peoples;
        networkManager.StartHost();

        Debug.Log("Host started without Steam. Max players: " + peoples);
    }

    public void GetLobbiesList()
    {
        Debug.Log("Lobby list is disabled because Steam matchmaking was removed.");
    }
}