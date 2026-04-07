using TMPro;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public string lobbyName;
    public string address;
    public TMP_Text lobbyTXT;

    public void SetLobbyData()
    {
        lobbyTXT.text = string.IsNullOrEmpty(lobbyName) ? "Empty" : lobbyName;
    }

    public void JoinLobby()
    {
        menuManager menu = FindObjectOfType<menuManager>();
        if (menu != null)
        {
            menu.JoinByAddress(address);
        }
    }
}