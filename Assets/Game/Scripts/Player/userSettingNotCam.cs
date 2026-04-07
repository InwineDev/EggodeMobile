using Mirror;
using System;
using TMPro;
using UnityEngine;

public class userSettingNotCam : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSkinChanged))]
    private int currentSkinIndex = 0;

    [SyncVar] public string Varible;
    [SyncVar] public int VaribleInt;
    [SyncVar] public int money;

    public GameObject effectMaslo;
    public bool uron = true;
    public TMP_Text hp;

    [Serializable]
    public struct EggData
    {
        public GameObject[] eggs;
        public Sprite[] eggSprites;
    }

    public EggData eggData;
    public userSettings us;
    public MessageText messageController;

    public TMP_Text Error228;
    public GameObject criterror;
    public GameObject spawn;
    public GameObject[] canvasi;

    public void StopGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }

    private void Start()
    {
        if (isOwned)
        {
            int sus = customdannie.id_skin;
            CmdChangeSkin(sus);
        }
    }

    public void ChangeSkin(int newSkinIndex)
    {
        if (isLocalPlayer)
        {
            CmdChangeSkin(newSkinIndex);
        }
    }

    private void OnSkinChanged(int oldValue, int newValue)
    {
        foreach (GameObject egg in eggData.eggs)
        {
            egg.SetActive(false);
        }

        eggData.eggs[newValue].SetActive(true);
        login.skindannaia = newValue;
    }

    public void Critical(string error)
    {
        criterror.SetActive(true);
        Error228.text = error;
    }

    public void OnSkinChangedNoMP(int newValue)
    {
        foreach (GameObject egg in eggData.eggs)
        {
            egg.SetActive(false);
        }

        eggData.eggs[newValue].SetActive(true);
        customdannie.id_skin = newValue;
    }

    [Command]
    private void CmdChangeSkin(int newSkinIndex)
    {
        currentSkinIndex = newSkinIndex;
    }

    public void one()
    {
        for (int i = 0; i < canvasi.Length; i++)
        {
            if (i != 1)
                canvasi[i].SetActive(false);

            if (i == 1)
                canvasi[i].transform.localPosition = new Vector3(0, 1500, 0);
        }
    }

    public void two()
    {
        canvasi[2].SetActive(true);
    }
}