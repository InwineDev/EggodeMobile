using UnityEngine;
using System.Collections.Generic;
using Mirror;
using System;
using Mirror.Experimental;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Rendering;

[System.Serializable]
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
    public List<string> modsDependence;
    public List<TextureData> textures;
    public List<MapObject> objects;
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
public class MapObject
{
    public string folderLocation;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public Color color;
    public bool isObject;
    public bool isMod;
    public bool isRigidbody;
    public bool isCollider;
    public bool isLomatel;
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
    public string node;
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
    public bool isLomatel;
    public bool isRigidbody;
    public bool isCollider;
    public string texture;
    public string textureTile;
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
    public int SetIntPlayerVarible;
    public string PlayerVaribleIf;
    public string PlayerVaribleIfMoreInt;
}

public class maploader : NetworkBehaviour
{
    public string jsonFilePath;
    public GameObject[] slider;
    
    void Start()
    {
        if (!isOwned) return;
            if (gameObject.name == "FirstPersonController [connId=0]" )
            {
                load();
            }
            else
            {
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    print("SHIT" + SceneManager.GetActiveScene().buildIndex);
                    FindObjectOfType<GameStatisticController>().buttonEvent("Зашёл к кому-то в игру");
                if(FindObjectOfType<serverProperties>().versionServer != menuManager.publicVersion)
                {
                    EError.error = "Версия несовместима. Версия сервера: " + FindObjectOfType<serverProperties>().versionServer + " Версия клиента " + menuManager.publicVersion;
                    GetComponent<userSettingNotCam>().StopGame();
                }
                if (FindObjectOfType<serverProperties>().serverMods.All(CsModsManager.modsForServer.Contains) & FindObjectOfType<serverProperties>().serverMods != null)
                {
                    Destroy(gameObject.GetComponent<maploader>());
                } else
                {
                    EError.error = "У вас не установлены все требуемые моды";
                    GetComponent<userSettingNotCam>().StopGame();
                }
                } else
                {
                    print("EEEE");
                }
            }
    }


    [TargetRpc]
    virtual public void load()
    {
        print("sus zagruzena karta by " + gameObject);
        jsonFilePath = login.urlMap;
        if (jsonFilePath == null) return;
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        MapData mapData = JsonUtility.FromJson<MapData>(jsonText);
        FindObjectOfType<serverProperties>().survival = mapData.survival;
        FindObjectOfType<serverProperties>().versionServer = menuManager.publicVersion;
        FindObjectOfType<GameStatisticController>().buttonEvent("Хостит карту " + mapData.mapname);
        try
        {
            foreach (var item1 in mapData.modsDependence)
            {
                FindObjectOfType<serverProperties>().serverMods.Add(item1);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }
        if (Int32.TryParse(mapData.skybox, out int skyboxValue))
        {
            FindObjectOfType<serverProperties>().skybox = skyboxValue;
        }
        else
        {
            Debug.LogError("Не удалось преобразовать skybox в целое число.");
        }

        if (String.IsNullOrEmpty(mapData.diecord))
        {
            FindObjectOfType<serverProperties>().dieCord = "0,2,0";
        }
        else
        {
            FindObjectOfType<serverProperties>().dieCord = mapData.diecord;
        }

        FindObjectOfType<serverProperties>().hp = mapData.uron;

        FindObjectOfType<serverProperties>().spawnn = mapData.canSpawnObj;

        FindObjectOfType<serverProperties>().destroy = mapData.canDellObj;

        StrahLoad(mapData);
    }
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

    virtual public void StrahLoad(MapData mapData)
    {
/*        foreach (var item in mapData.textures)
        {
            
        }*/
        foreach (MapObject mapObject in mapData.objects)
        {
            GameObject prefab = Resources.Load<GameObject>(mapObject.folderLocation);
            print(mapObject.folderLocation);
            if (loadBundle(mapObject.folderLocation) != null)
            {
                prefab = loadBundle(mapObject.folderLocation);
            }

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, mapObject.position, Quaternion.Euler(mapObject.rotation));
                obj.transform.localScale = mapObject.scale;
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    NetworkServer.Spawn(obj);
                }
                else
                {
                    Destroy(obj.GetComponent<NetworkIdentity>());
                    Destroy(obj.GetComponent<Rigidbody>());
                    obj.transform.parent = slider[0].transform;
                    
                }
                obj.GetComponent<name24>().sugoma224 = mapObject.color;
                obj.GetComponent<name24>().isLomatel = mapObject.isLomatel;
                try
                {
                    serverProperties.instance.allBlocks.Add(obj.GetComponent<name24>());
                }
                catch
                {

                }
                if (mapObject.isObject)
                {
                    obj.tag = "object";
                }
                if (mapObject.texture != null)
                {
                                        if (mapObject.texture.Contains("eggodetexture//"))
                                        {
                                            foreach (var item in mapData.textures)
                                            {
                                                if (mapObject.texture != "eggodetexture//" + item.nameoftexture) break;
                                                byte[] pvrtcBytes = item.bytes;
                            foreach (byte b in pvrtcBytes)
                            {
                                obj.GetComponent<name24>().bytesForTexture.Add(b);
                            }

                            obj.GetComponent<name24>().texture = mapObject.texture;
                        }
                                        }
                                        else
                                        {
                                            obj.GetComponent<name24>().texture = mapObject.texture;
                                        }
                }
                if(mapObject.textureTile != null) obj.GetComponent<name24>().textureTile = mapObject.textureTile;
                if (mapObject.isCollider)
                {
                    obj.GetComponent<name24>().isCollider = mapObject.isCollider;
                }
                if (mapObject.isRigidbody)
                {
                    obj.GetComponent<name24>().isRigidbody = mapObject.isRigidbody;
                }
                else
                {
                    Destroy(obj.GetComponent<NetworkRigidbodyReliable>());
                    Destroy(obj.GetComponent<NetworkRigidbodyUnreliable>());
                    Destroy(gameObject.GetComponent<NetworkRigidbody>());
                }
                obj.GetComponent<scriptor>().type = mapObject.type;
                if (mapObject.TpCord != null & mapObject.TpCord != "")
                {
                    obj.GetComponent<scriptor>().TpCord = mapObject.TpCord;
                }
                if (mapObject.id != null & mapObject.id != "")
                {
                    obj.GetComponent<name24>().id = mapObject.id;
                }
                if (mapObject.Animation != null & mapObject.Animation != "")
                {
                    obj.GetComponent<scriptor>().Animation = mapObject.Animation;
                }
                if (mapObject.Destroy != null & mapObject.Destroy != "")
                {
                    obj.GetComponent<scriptor>().Destroy = mapObject.Destroy;
                }
                if (mapObject.Damagenum != null & mapObject.Damagenum != "")
                {
                    obj.GetComponent<scriptor>().Damagenum = mapObject.Damagenum;
                }
                if (mapObject.Speed != null & mapObject.Speed != "")
                {
                    obj.GetComponent<scriptor>().Speed = mapObject.Speed;
                }
                if (mapObject.SetSize != null & mapObject.SetSize != "")
                {
                    obj.GetComponent<scriptor>().SetSize = mapObject.SetSize;
                }
                if (mapObject.Jump != null & mapObject.Jump != "")
                {
                    obj.GetComponent<scriptor>().Jump = mapObject.Jump;
                }
                if (mapObject.PlayAnim != null & mapObject.PlayAnim != "")
                {
                    obj.GetComponent<scriptor>().PlayAnim = mapObject.PlayAnim;
                }
                if (mapObject.II)
                {
                    obj.GetComponent<scriptor>().II = mapObject.II;
                }
                obj.GetComponent<scriptor>().SetPlayerVarible = mapObject.SetPlayerVarible;
                obj.GetComponent<scriptor>().SetIntPlayerVarible = mapObject.SetIntPlayerVarible;
                obj.GetComponent<scriptor>().PlayerVaribleIf = mapObject.PlayerVaribleIf;
                obj.GetComponent<scriptor>().PlayerVaribleIfMoreInt = mapObject.PlayerVaribleIfMoreInt;
                obj.GetComponent<scriptor>().AddItem = mapObject.AddItem;
                obj.GetComponent<scriptor>().nodesCode = mapObject.node;
                MetodMods(obj);
            }

        }
/*
        foreach (NPCData mapObject in mapData.npc)
        {
            GameObject prefab = Resources.Load<GameObject>(mapObject.folderLocation);

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, mapObject.position, Quaternion.Euler(mapObject.rotation));
                obj.transform.localScale = mapObject.scale;
                NetworkServer.Spawn(obj);
                obj.GetComponent<name24>().sugoma224 = mapObject.color;
                if (mapObject.isObject)
                {
                    obj.tag = "object";
                }
                if (mapObject.texture != null)
                {
                    obj.GetComponent<name24>().texture = mapObject.texture;
                }
                if (mapObject.isCollider)
                {
                    obj.GetComponent<name24>().isCollider = mapObject.isCollider;
                }
                if (mapObject.isRigidbody)
                {
                    obj.GetComponent<name24>().isRigidbody = mapObject.isRigidbody;
                }
                else
                {
                    Destroy(obj.GetComponent<NetworkRigidbodyReliable>());
                    Destroy(obj.GetComponent<NetworkRigidbodyUnreliable>());
                }
                if (mapObject.TpCord != null & mapObject.TpCord != "")
                {
                    obj.GetComponent<scriptor>().TpCord = mapObject.TpCord;
                }
                if (mapObject.id != null & mapObject.id != "")
                {
                    obj.GetComponent<name24>().id = mapObject.id;
                }
                if (mapObject.Animation != null & mapObject.Animation != "")
                {
                    obj.GetComponent<scriptor>().Animation = mapObject.Animation;
                }
                if (mapObject.Destroy != null & mapObject.Destroy != "")
                {
                    obj.GetComponent<scriptor>().Destroy = mapObject.Destroy;
                }
                if (mapObject.Damagenum != null & mapObject.Damagenum != "")
                {
                    obj.GetComponent<scriptor>().Damagenum = mapObject.Damagenum;
                }
                if (mapObject.Speed != null & mapObject.Speed != "")
                {
                    obj.GetComponent<scriptor>().Speed = mapObject.Speed;
                }
                if (mapObject.SetSize != null & mapObject.SetSize != "")
                {
                    obj.GetComponent<scriptor>().SetSize = mapObject.SetSize;
                }
                if (mapObject.PlayAnim != null & mapObject.PlayAnim != "")
                {
                    obj.GetComponent<scriptor>().PlayAnim = mapObject.PlayAnim;
                }
                if (mapObject.II)
                {
                    obj.GetComponent<scriptor>().II = mapObject.II;
                }
                if (!String.IsNullOrEmpty(mapObject.npcslovo))
                {
                    obj.GetComponent<npcController>().npcReplics = mapObject.npcslovo;
                }
                obj.GetComponent<scriptor>().SetPlayerVarible = mapObject.SetPlayerVarible;
                obj.GetComponent<scriptor>().PlayerVaribleIf = mapObject.PlayerVaribleIf;
            }
        }*/
    }

    virtual public void MetodMods(GameObject obj)
    {

    }
}
