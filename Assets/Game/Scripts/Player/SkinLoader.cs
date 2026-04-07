using Mirror;
using UnityEngine;

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

    [SyncVar(hook = nameof(ChangeBody))] public int body;
    [SyncVar(hook = nameof(ChangeNose))] public int nose;
    [SyncVar(hook = nameof(ChangeMouth))] public int mouth;
    [SyncVar(hook = nameof(ChangeEye))] public int eye;
    [SyncVar(hook = nameof(ChangeHat))] public int hat;

    private void Start()
    {
        if (isLocalPlayer)
            Load();
        else
            ApplySyncVars();
    }

    public void Load()
    {
        SkinData skinData = ParseAndClamp(UserContentPaths.LoadSkinJsonOrDefault());
        CmdSetSkin(skinData);
        LoadSkin(skinData);
    }

    public void LocalLoad()
    {
        SkinData skinData = ParseAndClamp(UserContentPaths.LoadSkinJsonOrDefault());
        LoadSkin(skinData);
    }

    [Command]
    private void CmdSetSkin(SkinData skindata)
    {
        body = ClampIndex(skindata.body, bodies);
        nose = ClampIndex(skindata.nose, noses);
        mouth = ClampIndex(skindata.mouth, mouthes);
        eye = ClampIndex(skindata.eye, eyes);
        hat = ClampIndex(skindata.hat, hats);
    }

    private SkinData ParseAndClamp(string json)
    {
        SkinData skinData = string.IsNullOrWhiteSpace(json) ? new SkinData() : JsonUtility.FromJson<SkinData>(json);
        if (skinData == null)
            skinData = new SkinData();

        skinData.body = ClampIndex(skinData.body, bodies);
        skinData.nose = ClampIndex(skinData.nose, noses);
        skinData.mouth = ClampIndex(skinData.mouth, mouthes);
        skinData.eye = ClampIndex(skinData.eye, eyes);
        skinData.hat = ClampIndex(skinData.hat, hats);
        return skinData;
    }

    private void ApplySyncVars()
    {
        ChangeBody(0, body);
        ChangeNose(0, nose);
        ChangeMouth(0, mouth);
        ChangeEye(0, eye);
        ChangeHat(0, hat);
    }

    private static int ClampIndex(int value, SkinDataObjectData[] source)
    {
        if (source == null || source.Length == 0)
            return 0;

        return Mathf.Clamp(value, 0, source.Length - 1);
    }

    private void LoadSkin(SkinData skindata)
    {
        SetActiveOnly(bodies, skindata.body);
        SetActiveOnly(noses, skindata.nose);
        SetActiveOnly(mouthes, skindata.mouth);
        SetActiveOnly(eyes, skindata.eye);
        SetActiveOnly(hats, skindata.hat);
    }

    private void ChangeBody(int oldValue, int newValue) => SetActiveOnly(bodies, newValue);
    private void ChangeNose(int oldValue, int newValue) => SetActiveOnly(noses, newValue);
    private void ChangeMouth(int oldValue, int newValue) => SetActiveOnly(mouthes, newValue);
    private void ChangeEye(int oldValue, int newValue) => SetActiveOnly(eyes, newValue);
    private void ChangeHat(int oldValue, int newValue) => SetActiveOnly(hats, newValue);

    private void SetActiveOnly(SkinDataObjectData[] array, int activeIndex)
    {
        if (array == null || array.Length == 0)
            return;

        int clampedIndex = Mathf.Clamp(activeIndex, 0, array.Length - 1);
        foreach (var entry in array)
        {
            if (entry?.skin != null)
                entry.skin.SetActive(false);
        }

        if (array[clampedIndex]?.skin != null)
            array[clampedIndex].skin.SetActive(true);
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
