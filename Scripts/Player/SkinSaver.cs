using Mirror.Examples.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SkinSaver : MonoBehaviour
{
    [SerializeField] private SkinLoader skinLoader;
    [SerializeField] private GameObject bodyList;
    [SerializeField] private GameObject noseList;
    [SerializeField] private GameObject mouthList;
    [SerializeField] private GameObject eyeList;
    [SerializeField] private GameObject hatList;
    [SerializeField] private GameObject skinPrefab;

    public Action<int, string> changeSkin;

    private void OnEnable()
    {
        changeSkin += ChangeSkin;
        for (int i = 0; i < skinLoader.bodies.Length; i++)
        {
            LoadSkin(i, "body", bodyList, skinLoader.bodies[i].skinSprite);
        }
        for (int i = 0; i < skinLoader.noses.Length; i++)
        {
            LoadSkin(i, "nose", noseList, skinLoader.noses[i].skinSprite);
        }
        for (int i = 0; i < skinLoader.mouthes.Length; i++)
        {
            LoadSkin(i, "mouth", mouthList, skinLoader.mouthes[i].skinSprite);
        }
        for (int i = 0; i < skinLoader.eyes.Length; i++)
        {
            LoadSkin(i, "eye", eyeList, skinLoader.eyes[i].skinSprite);
        }
        for (int i = 0; i < skinLoader.hats.Length; i++)
        {
            LoadSkin(i, "hat", hatList, skinLoader.hats[i].skinSprite);
        }
    }

    private void LoadSkin(int id, string type, GameObject parent, Sprite sprite)
    {
        GameObject strah = Instantiate(skinPrefab);
        strah.transform.SetParent(parent.transform, false);
        SkinDataObject skinData = strah.GetComponent<SkinDataObject>();
        skinData.id = id;
        skinData.skinSaver = this;
        skinData.type = type;
        skinData.preview.sprite = sprite;
    }
    private void OnDisable()
    {
        changeSkin -= ChangeSkin;
    }

    public void ChangeSkin(int skinNumber, string skinType)
    {
        string jsonFilePath = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Skin.eggodeskin");
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        SkinData skinData = JsonUtility.FromJson<SkinData>(jsonText);

        switch (skinType)
        {
            case "body":
                skinData.body = skinNumber;
                break;
            case "nose":
                skinData.nose = skinNumber;
                break;
            case "mouth":
                skinData.mouth = skinNumber;
                break;
            case "eye":
                skinData.eye = skinNumber;
                break;
            case "hat":
                skinData.hat = skinNumber;
                break;
        }
        string json = JsonUtility.ToJson(skinData, true);

        File.WriteAllText(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/") + "Skin.eggodeskin", json);
        skinLoader.LocalLoad();
    }

    public void RandomSkin()
    {
        string jsonFilePath = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Skin.eggodeskin");
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        SkinData skinData = JsonUtility.FromJson<SkinData>(jsonText);

        skinData.body = UnityEngine.Random.Range(0, skinLoader.bodies.Length);

        skinData.nose = UnityEngine.Random.Range(0, skinLoader.noses.Length);

        skinData.mouth = UnityEngine.Random.Range(0, skinLoader.mouthes.Length);

        skinData.eye = UnityEngine.Random.Range(0, skinLoader.eyes.Length);
        skinData.hat = UnityEngine.Random.Range(0, skinLoader.hats.Length);

        string json = JsonUtility.ToJson(skinData, true);

        File.WriteAllText(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/") + "Skin.eggodeskin", json);
        skinLoader.LocalLoad();
    }
}
