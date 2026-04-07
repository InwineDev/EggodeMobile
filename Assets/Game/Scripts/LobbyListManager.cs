using System.Collections.Generic;
using UnityEngine;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;

    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject lobbiesButton;
    public GameObject hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        lobbiesMenu.SetActive(true);
        Debug.Log("Steam lobby list removed. Use direct IP connection instead.");
    }

    public void AddLobby(string lobbyName, string address)
    {
        if (lobbyDataItemPrefab == null || lobbyListContent == null)
            return;

        GameObject createdItem = Instantiate(lobbyDataItemPrefab, lobbyListContent.transform);
        createdItem.transform.localScale = Vector3.one;

        LobbyData data = createdItem.GetComponent<LobbyData>();
        if (data != null)
        {
            data.lobbyName = lobbyName;
            data.address = address;
            data.SetLobbyData();
        }

        listOfLobbies.Add(createdItem);
    }

    public void DestroyLobbies()
    {
        foreach (GameObject item in listOfLobbies)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        listOfLobbies.Clear();
    }
}