using UnityEngine;
using TMPro;
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

    public override void load()
    {
        Debug.Log("IMPORT MAP");
        ConsoleController.cc.AddMessage("Importing map " + sus.text, 0);

        jsonFilePath = Path.Combine(UserContentPaths.MapsPersistent, sus.text + ".eggodemap");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogWarning("Map file not found: " + jsonFilePath);
            return;
        }

        string jsonText = File.ReadAllText(jsonFilePath);
        MapData mapData = JsonUtility.FromJson<MapData>(jsonText);

        if (mapData == null)
            return;

        if (Int32.TryParse(mapData.skybox, out int skyboxValue))
        {
            FindObjectOfType<SaveMapNotStart>().SetSkyBox(skyboxValue);
            mozg[0].GetComponent<TMP_Dropdown>().value = skyboxValue;
        }

        if (!string.IsNullOrEmpty(mapData.diecord))
            mozg[1].GetComponent<TMP_InputField>().text = mapData.diecord;

        serverPropertes[0].isOn = mapData.uron;
        serverPropertes[1].isOn = mapData.canSpawnObj;
        serverPropertes[2].isOn = mapData.canDellObj;
        serverPropertes[3].isOn = mapData.survival;

        icon.text = mapData.icon;
        gameObject.GetComponent<Button>().interactable = false;

        if (mapData.textures != null)
        {
            foreach (var item in mapData.textures)
                ttl.loadTexture(item.nameoftexture, item.bytes, item.width, item.height);
        }

        StrahLoad(mapData);
    }

    public override void StrahLoad(MapData mapData)
    {
        base.StrahLoad(mapData);
    }
}
