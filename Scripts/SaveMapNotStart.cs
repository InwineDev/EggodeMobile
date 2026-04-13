using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Mirror;
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
    public Toggle Tog;
    public Toggle Tog2;
    public Toggle Tog3;
    public TMP_InputField icon;

    [SerializeField] private textureList tt;

    public bool[] parametrs = {true, true, true};

    public Toggle[] parametrsTrue;

    [SerializeField]
    private Material[] skyboxes;

    [SerializeField] private Volume skyVolume;

    private string diecord = "0,2,0";

    public TMP_Text txtDieCord;

    public GameObject error;

    public TMP_Dropdown type;
    [SerializeField] private ModsDependences mdc;


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
        SaveMapDataExit();
        Invoke("ShowError", 5f);
        gameObject.GetComponent<Button>().interactable = false;
    }


    public IEnumerator autosave()
    {
        objects.Clear();
        npc.Clear();
        yield return new WaitForSeconds(145f);
        mapName = "autosave";
        author = settingsController.nickname;
        AddChildrenOfDocherToObjects();
        ConsoleController.cc.AddMessage("Autosave...", 0);
        SaveMapData("autosave/");
        StartCoroutine(autosave());
    }
    private void ShowError()
    {
        /*error.SetActive(true);*/
    }

    private void AddChildrenOfDocherToObjects()
    {
        foreach (Transform child in docher.transform)
        {
            objects.Add(child.gameObject);
        }
        foreach (Transform child in docher2.transform)
        {
            npc.Add(child.gameObject);
        }
    }

    public void SetDiecord(string newv)
    {
        diecord = newv;
    }

    public void SetParamentr1(bool newPar)
    {
        parametrs[1] = newPar;
    }
    public void SetParamentr0(bool newPar)
    {
        parametrs[0] = newPar;
    }
    public void SetParamentr2(bool newPar)
    {
        parametrs[2] = newPar;
    }

    public void SaveMapData(string way)
    {
        ConsoleController.cc.AddMessage("Start export...", 0);
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
        ConsoleController.cc.AddMessage("Exported base data!", 0);
        List<TextureData> textureData = new List<TextureData>();
        List<string> modsDependences = new List<string>();
        foreach (var item1 in mdc.dcs)
        {
            modsDependences.Add(item1.tmpif.text);
        }
        foreach (textureController obj in tt.textures)
        {
            TextureData data = new TextureData();
            data.bytes = obj.textureBytes;
            data.nameoftexture = obj.myname;
            data.width = obj.width;
            data.height = obj.height;

            textureData.Add(data);
            ConsoleController.cc.AddMessage("Exported texture '" + obj.myname + "'", 0);
        }

        List<MapObject> objectData = new List<MapObject>();

        foreach (GameObject obj in objects)
        {
            MapObject data = new MapObject();
            data.folderLocation = obj.GetComponent<name24>().name244;
            data.position = obj.transform.position;
            data.rotation = obj.transform.localEulerAngles;
            data.scale = obj.transform.localScale;
            if (obj.GetComponent<Renderer>())
            {
                data.color = obj.GetComponent<Renderer>().material.color;
            } else
            {
                data.color = new Color(10f,10f,10f);
            }
            data.isObject = obj.GetComponent<name24>().isLomatel;
            data.isRigidbody = obj.GetComponent<name24>().isRigidbody;
            data.isCollider = obj.GetComponent<name24>().isCollider;
            data.texture = obj.GetComponent<name24>().texture;
            data.textureTile = obj.GetComponent<name24>().textureTile;
            data.type = obj.GetComponent<scriptor>().type;
            data.id = obj.GetComponent<name24>().id;
            data.Damagenum = obj.GetComponent<scriptor>().Damagenum;
            data.TpCord = obj.GetComponent<scriptor>().TpCord;
            data.Animation = obj.GetComponent<scriptor>().Animation;
            data.PlayAnim = obj.GetComponent<scriptor>().PlayAnim;
            data.Destroy = obj.GetComponent<scriptor>().Destroy;
            data.Speed = obj.GetComponent<scriptor>().Speed;
            data.Jump = obj.GetComponent<scriptor>().Jump;
            data.SetSize = obj.GetComponent<scriptor>().SetSize;
            data.II = obj.GetComponent<scriptor>().II;
            data.SetPlayerVarible = obj.GetComponent<scriptor>().SetPlayerVarible;
            data.PlayerVaribleIf = obj.GetComponent<scriptor>().PlayerVaribleIf;
            data.AddItem = obj.GetComponent<scriptor>().AddItem;
            data.SetIntPlayerVarible = obj.GetComponent<scriptor>().SetIntPlayerVarible;
            data.PlayerVaribleIfMoreInt = obj.GetComponent<scriptor>().PlayerVaribleIfMoreInt;
            data.node = obj.GetComponent<scriptor>().nodesCode;

            objectData.Add(data);
            ConsoleController.cc.AddMessage("Exported object '" + obj.name + "'", 0);
        }

        mapData.textures = textureData;
        mapData.modsDependence = modsDependences;
        mapData.objects = objectData;

        List<NPCData> NPCData = new List<NPCData>();

        foreach (GameObject obj in npc)
        {
            NPCData data = new NPCData();
            data.folderLocation = obj.GetComponent<name24>().name244;
            data.position = obj.transform.position;
            data.rotation = obj.transform.localEulerAngles;
            data.scale = obj.transform.localScale;
            data.color = obj.GetComponent<Renderer>().material.color;
            data.isObject = obj.GetComponent<name24>().isLomatel;
            data.isRigidbody = obj.GetComponent<name24>().isRigidbody;
            data.isCollider = obj.GetComponent<name24>().isCollider;
            data.texture = obj.GetComponent<name24>().texture;
            data.id = obj.GetComponent<name24>().id;
            data.Damagenum = obj.GetComponent<scriptor>().Damagenum;
            data.TpCord = obj.GetComponent<scriptor>().TpCord;
            data.Animation = obj.GetComponent<scriptor>().Animation;
            data.PlayAnim = obj.GetComponent<scriptor>().PlayAnim;
            data.Destroy = obj.GetComponent<scriptor>().Destroy;
            data.Speed = obj.GetComponent<scriptor>().Speed;
            data.Jump = obj.GetComponent<scriptor>().Jump;
            data.SetSize = obj.GetComponent<scriptor>().SetSize;
            data.II = obj.GetComponent<scriptor>().II;
            data.npcslovo = obj.GetComponent<npcController>().npcReplics;
            data.SetPlayerVarible = obj.GetComponent<scriptor>().SetPlayerVarible;
            data.PlayerVaribleIf = obj.GetComponent<scriptor>().PlayerVaribleIf;

            NPCData.Add(data);
            ConsoleController.cc.AddMessage("Exported NPC '" + obj.name + "'", 0);
        }


        mapData.npc = NPCData;


        string json = JsonUtility.ToJson(mapData, true);
        print(Path.Combine(Path.GetDirectoryName(Application.dataPath), way) + mapName + ".eggodemap");


        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), way) + mapName + ".eggodemap", json);
        ConsoleController.cc.AddMessage("Map exported! Use 'Exit' to exit!", 0);
    }
    public void SaveMapDataExit()
    {
/*        if (NetworkServer.active && NetworkClient.isConnected)
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
        }*/
    }

        public void SetSkyBox(int newValue)
    {
        skybox = newValue.ToString();
        /*VisualEnvironment visualEnv;
        HDRISky hdriSky;

        if (!skyVolume.profile.TryGet(out visualEnv))
            visualEnv = skyVolume.profile.Add<VisualEnvironment>(false);

        if (!skyVolume.profile.TryGet(out hdriSky))
            hdriSky = skyVolume.profile.Add<HDRISky>(false);

        // Change the sky type to HDRI Sky
        visualEnv.skyType.value = (int)SkyType.HDRI;

        // Change the HDRISky component's texture and intensity
        hdriSky.hdriSky.value = skyboxes[newValue];
        hdriSky.exposure.value = 16f; // Example exposure value
        hdriSky.multiplier.value = 1f; // Example intensity multiplier*/
        RenderSettings.skybox = skyboxes[newValue];
    }

/*    [System.Serializable]
    public class MapData
    {
        public string mapname;
        public string author;
        public string icon;
        public string skybox;
        public string diecord;
        public bool uron = true;
        public bool canSpawnObj = true;
        public bool canDellObj = true;
        public bool survival;
        public List<TextureData> textures;
        public List<ObjectData> objects;
        public List<NPCData> npc;
    }

    [System.Serializable]
    public class TextureData
    {
        public string nameoftexture;
        public byte[] bytes;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class ObjectData
    {
        public string folderLocation;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Color color;
        public bool isObject;
        public bool isRigidbody;
        public bool isCollider;
        public string texture;
        public string textureTile;
        public int type;
        public string TpCord;
        public string Animation;
        public string PlayAnim;
        public string Destroy;
        public string id;
        public string Damagenum;
        public string Speed;
        public string Jump;
        public string SetSize;
        public bool II;
        public string SetPlayerVarible;
        public string PlayerVaribleIf;
        public string AddItem;
        public string PlayerVaribleIfMoreInt;
        public int SetIntPlayerVarible;
    }

    [System.Serializable]
    public class NPCData
    {
        public string folderLocation;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Color color;
        public bool isObject;
        public bool isRigidbody;
        public bool isCollider;
        public string texture;
        public string TpCord;
        public string Animation;
        public string PlayAnim;
        public string Destroy;
        public string id;
        public string Damagenum;
        public string Speed;
        public string Jump;
        public string SetSize;
        public bool II;
        public string npcslovo;
        public string SetPlayerVarible;
        public string PlayerVaribleIf;
    }*/

}