using Mirror;
using Mirror.Examples.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkinDataObjectData
{
    public GameObject skin;
    public Sprite skinSprite;
}

public class SkinLoader : NetworkBehaviour
{
    public SkinDataObjectData[] bodies;
    public SkinDataObjectData[] noses;
    public SkinDataObjectData[] mouthes;
    public SkinDataObjectData[] eyes;
    public SkinDataObjectData[] hats;

    [SyncVar(hook = nameof(ChangeBody))]
    public int body;

    [SyncVar(hook = nameof(ChangeNose))]
    public int nose;

    [SyncVar(hook = nameof(ChangeMouth))]
    public int mouth;

    [SyncVar(hook = nameof(ChangeEye))]
    public int eye;

    [SyncVar(hook = nameof(ChangeHat))]
    public int hat;

    void Start()
    {
        if (isLocalPlayer)
        {
            Load();
        }
        else
        {
            // Apply the current SyncVar values for remote players
            ChangeBody(0, body);
            ChangeNose(0, nose);
            ChangeMouth(0, mouth);
            ChangeEye(0, eye);
            ChangeHat(0, hat);
        }
    }

    public void Load()
    {
        string jsonFilePath = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Skin.eggodeskin");
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        SkinData skinData = JsonUtility.FromJson<SkinData>(jsonText);

        // Call the command to update the skin on the server
        CmdSetSkin(skinData);

        // Also update locally immediately (optional, depends on your needs)
        LoadSkin(skinData);
    }

    [Command]
    void CmdSetSkin(SkinData skindata)
    {
        // These SyncVar changes will be propagated to all clients
        body = skindata.body;
        nose = skindata.nose;
        mouth = skindata.mouth;
        eye = skindata.eye;
        hat = skindata.hat;
    }
    public void LocalLoad()
    {
        string jsonFilePath = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Skin.eggodeskin");
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        SkinData skinData = JsonUtility.FromJson<SkinData>(jsonText);

        LoadSkin(skinData);
    }

    /*[ClientRpc]
    void RpcSetSkin(SkinData skindata)
    {
        LoadSkin(skindata);
    }*/

    void LoadSkin(SkinData skindata)
    {
        foreach (var item1 in bodies)
        {
            item1.skin.SetActive(false);
        }
        bodies[skindata.body].skin.SetActive(true);

        foreach (var item1 in noses)
        {
            item1.skin.SetActive(false);
        }
        noses[skindata.nose].skin.SetActive(true);

        foreach (var item1 in mouthes)
        {
            item1.skin.SetActive(false);
        }
        mouthes[skindata.mouth].skin.SetActive(true);

        foreach (var item1 in eyes)
        {
            item1.skin.SetActive(false);
        }
        eyes[skindata.eye].skin.SetActive(true);

        foreach (var item1 in hats)
        {
            item1.skin.SetActive(false);
        }
        hats[skindata.hat].skin.SetActive(true);
    }


    void ChangeBody(int old, int neww)
    {
        foreach (var item1 in bodies)
        {
            item1.skin.SetActive(false);
        }
        bodies[neww].skin.SetActive(true);
    }
    void ChangeNose(int old, int neww)
    {
        foreach (var item1 in noses)
        {
            item1.skin.SetActive(false);
        }
        noses[neww].skin.SetActive(true);
    }
    void ChangeMouth(int old, int neww)
    {
        foreach (var item1 in mouthes)
        {
            item1.skin.SetActive(false);
        }
        mouthes[neww].skin.SetActive(true);
    }
    void ChangeEye(int old, int neww)
    {
        foreach (var item1 in eyes)
        {
            item1.skin.SetActive(false);
        }
        eyes[neww].skin.SetActive(true);
    }

    void ChangeHat(int old, int neww)
    {
        foreach (var item1 in hats)
        {
            item1.skin.SetActive(false);
        }
        hats[neww].skin.SetActive(true);
    }
}
[System.Serializable]
public class SkinData
{
    public int body;
    public int nose;
    public int mouth;
    public int eye;
    public int hat;
}
