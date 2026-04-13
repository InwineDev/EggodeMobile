using Mirror;
using TMPro;
using UnityEngine;

public class menuManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public TMP_InputField address;
    public GameObject openmama;
    public GameObject LobbyListSUS;
    public string version;
    public static string publicVersion;
    public TMP_Text versionText;
    public GameObject selectMap;

    private void Awake()
    {
        publicVersion = version;
        versionText.text = publicVersion;
        Cursor.lockState = CursorLockMode.None;
    }

    public void openmamavoid()
    {
        openmama.SetActive(true);
    }

    public void closemamavoid()
    {
        openmama.SetActive(false);
    }

    public void openmamasvoid()
    {
        LobbyListSUS.SetActive(true);
    }

    public void closemamasvoid()
    {
        LobbyListSUS.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void JoinClient()
    {
        string[] address_code = address.text.Split(',');

        if (address_code.Length == 4)
        {
            int part0 = int.Parse(address_code[0]);
            int part1 = int.Parse(address_code[1]);
            int part2 = int.Parse(address_code[2]);
            int part3 = int.Parse(address_code[3]);

            int newPart0 = (part0 - 641) / 2;
            int newPart1 = (part1 - 641) / 2;
            int newPart2 = (part2 - 641) / 2;
            int newPart3 = (part3 - 641) / 2;

            networkManager.networkAddress = $"{newPart0}.{newPart1}.{newPart2}.{newPart3}";
            networkManager.StartClient();
        }
        else
        {
            networkManager.networkAddress = string.IsNullOrWhiteSpace(address.text)
                ? "localhost"
                : address.text;

            networkManager.StartClient();
        }
    }

    public void JoinByAddress(string ipAddress)
    {
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

    private void OnEnable()
    {
        networkManager.StopClient();
        networkManager.StopHost();
    }

    public void HostGame()
    {
        selectMap.SetActive(true);
    }
}