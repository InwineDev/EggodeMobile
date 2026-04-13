using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using System.Collections;
using Mirror;
using UnityEngine.UI;
using System;
using System.IO;

public class maploadercreator : maploader
{

    public TMP_InputField sus;
    public Toggle[] serverPropertes;
    public GameObject image1;
    public TMP_InputField icon;

    public textureList ttl;

    public GameObject[] mozg;

    public TMP_Dropdown dropType;

    GameObject loadBundle(string path)
    {
        foreach (GameObject item in FindObjectOfType<ModLoader>().objectsMapEditor)
        {
            if (item.GetComponent<name24>().name244 == path)
            {
                return item;
            }
        }
        return null;
    }

    public override void load()
    {
        print("OMG ZAGRUZKA");
        ConsoleController.cc.AddMessage("Importing map " + sus.text, 0);
        jsonFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "maps/") + sus.text + ".eggodemap";
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        MapData mapData = JsonUtility.FromJson<MapData>(jsonText);

        if (Int32.TryParse(mapData.skybox, out int skyboxValue))
        {
            FindObjectOfType<SaveMapNotStart>().SetSkyBox(skyboxValue);
            //image1.SetActive(true);
            mozg[0].GetComponent<TMP_Dropdown>().value = skyboxValue;
        }
        if (!String.IsNullOrEmpty(mapData.diecord))
        {
            //image1.SetActive(true);
            mozg[1].GetComponent<TMP_InputField>().text = mapData.diecord;
        }
        try
        {
            foreach (var item1 in mapData.modsDependence)
            {
                FindObjectOfType<ModsDependences>().AddDependences(item1);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }

        serverPropertes[0].isOn = mapData.uron;
        serverPropertes[1].isOn = mapData.canSpawnObj;
        serverPropertes[2].isOn = mapData.canDellObj;
        serverPropertes[3].isOn = mapData.survival;

        icon.text = mapData.icon;
        gameObject.GetComponent<Button>().interactable = false;

        foreach (var item in mapData.textures)
        {
            ttl.loadTexture(item.nameoftexture, item.bytes, item.width, item.height);
        }
        StrahLoad(mapData);
    }

    public override void StrahLoad(MapData mapData)
    {
        base.StrahLoad(mapData);
    }

    IEnumerator Pon(string texture2, GameObject sus)
    {
        print(texture2);
        WWW www = new WWW(texture2);
        yield return www;

        if (www.error == null)
        {
            print("otpariv");
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.ARGB32, false);
            www.LoadImageIntoTexture(texture);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

            print("poluchil");

            sus.GetComponent<Renderer>().material.mainTexture = texture;

            print("postavil");

        }
        else
        {
            print("Error loading image: " + www.error);
        }
    }
}
