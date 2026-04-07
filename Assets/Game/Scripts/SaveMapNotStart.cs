using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;

public class SaveMapNotStart : MonoBehaviour
{
    public string mapName;
    public TMP_InputField sugoming;
    public string author;
    public string skybox;
    public List<GameObject> objects;
    public List<GameObject> npc;
    public GameObject docher;
    public GameObject docher2;
    public Toggle[] parametrsTrue;
    public TMP_InputField icon;

    [SerializeField] private textureList tt;
    [SerializeField] private Material[] skyboxes;
    [SerializeField] private Volume skyVolume;

    private string diecord = "0,2,0";
    public TMP_Text txtDieCord;

    public TMP_Dropdown type;

    public void Start()
    {
        StartCoroutine(autosave());
    }

    public void Saving()
    {
        objects.Clear();
        npc.Clear();
        mapName = sugoming.text;
        author = settingsController.nickname;

        AddChildrenOfDocherToObjects();

        SaveMapData("maps/");
        Invoke("DisableButton", 1f);
    }

    private void DisableButton()
    {
        GetComponent<Button>().interactable = false;
    }

    public IEnumerator autosave()
    {
        yield return new WaitForSeconds(145f);

        objects.Clear();
        npc.Clear();

        mapName = "autosave";
        author = settingsController.nickname;

        AddChildrenOfDocherToObjects();

        SaveMapData("autosave/");

        StartCoroutine(autosave());
    }

    private void AddChildrenOfDocherToObjects()
    {
        foreach (Transform child in docher.transform)
            objects.Add(child.gameObject);

        foreach (Transform child in docher2.transform)
            npc.Add(child.gameObject);
    }

    public void SaveMapData(string way)
    {
        MapData mapData = new MapData();

        diecord = txtDieCord.text;

        mapData.mapname = mapName;
        mapData.author = author;
        mapData.icon = icon.text;
        mapData.skybox = skybox;
        mapData.diecord = diecord;

        mapData.uron = parametrsTrue[0].isOn;
        mapData.canSpawnObj = parametrsTrue[1].isOn;
        mapData.canDellObj = parametrsTrue[2].isOn;
        mapData.survival = parametrsTrue[3].isOn;

        List<TextureData> textureData = new List<TextureData>();

        foreach (textureController obj in tt.textures)
        {
            TextureData data = new TextureData();
            data.bytes = obj.textureBytes;
            data.nameoftexture = obj.myname;
            data.width = obj.width;
            data.height = obj.height;

            textureData.Add(data);
        }

        List<MapObject> objectData = new List<MapObject>();

        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            MapObject data = new MapObject();

            var name = obj.GetComponent<name24>();
            var script = obj.GetComponent<scriptor>();

            if (name == null || script == null) continue;

            data.folderLocation = name.name244;
            data.position = obj.transform.position;
            data.rotation = obj.transform.localEulerAngles;
            data.scale = obj.transform.localScale;

            if (obj.GetComponent<Renderer>())
                data.color = obj.GetComponent<Renderer>().material.color;

            data.isObject = name.isLomatel;
            data.isRigidbody = name.isRigidbody;
            data.isCollider = name.isCollider;
            data.texture = name.texture;
            data.textureTile = name.textureTile;
            data.type = script.type;
            data.id = name.id;

            objectData.Add(data);
        }

        mapData.textures = textureData;
        mapData.objects = objectData;

        string json = JsonUtility.ToJson(mapData, true);

        string path = Path.Combine(Application.persistentDataPath, way);
        Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, mapName + ".eggodemap"), json);
    }

    public void SetSkyBox(int newValue)
    {
        skybox = newValue.ToString();
        RenderSettings.skybox = skyboxes[newValue];
    }
}