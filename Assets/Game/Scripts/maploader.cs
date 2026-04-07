using UnityEngine;
using System.Collections.Generic;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.IO;

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

    private void Start()
    {
        if (!isOwned) return;

        if (gameObject.name == "FirstPersonController [connId=0]")
        {
            load();
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                GameStatisticController stat = FindObjectOfType<GameStatisticController>();
                if (stat != null)
                    stat.buttonEvent("Зашёл в игру");

                serverProperties sp = FindObjectOfType<serverProperties>();
                userSettingNotCam user = GetComponent<userSettingNotCam>();

                if (sp != null && sp.versionServer != menuManager.publicVersion)
                {
                    EError.error = "Версия не совпадает. Версия сервера: " +
                                   sp.versionServer +
                                   " Версия клиента: " + menuManager.publicVersion;

                    if (user != null)
                        user.StopGame();
                }
                else
                {
                    Destroy(GetComponent<maploader>());
                }
            }
        }
    }

    [TargetRpc]
    public virtual void load()
    {
        Debug.Log("load map by " + gameObject);

        string jsonText = null;
        jsonFilePath = login.urlMap;

        try
        {
            if (!string.IsNullOrEmpty(SelectedMapState.EmbeddedMapJson))
            {
                jsonText = SelectedMapState.EmbeddedMapJson;
            }
            else if (!string.IsNullOrEmpty(SelectedMapState.PersistentMapPath) && File.Exists(SelectedMapState.PersistentMapPath))
            {
                jsonFilePath = SelectedMapState.PersistentMapPath;
                jsonText = File.ReadAllText(SelectedMapState.PersistentMapPath);
            }
            else if (!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
            {
                jsonText = File.ReadAllText(jsonFilePath);
            }
            else if (!string.IsNullOrEmpty(SelectedMapState.ResourcesMapPath))
            {
                TextAsset mapAsset = Resources.Load<TextAsset>(SelectedMapState.ResourcesMapPath);
                if (mapAsset != null)
                    jsonText = mapAsset.text;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to read map json: " + ex.Message);
            return;
        }

        if (string.IsNullOrWhiteSpace(jsonText))
        {
            Debug.LogError("Map json is empty or missing.");
            return;
        }

        MapData mapData;
        try
        {
            mapData = JsonUtility.FromJson<MapData>(jsonText);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to parse map json: " + ex.Message);
            Debug.LogError("Map json preview: " + jsonText.Substring(0, Mathf.Min(jsonText.Length, 300)));
            return;
        }

        if (mapData == null) return;

        serverProperties sp = FindObjectOfType<serverProperties>();
        if (sp == null) return;

        sp.survival = mapData.survival;
        sp.versionServer = menuManager.publicVersion;

        GameStatisticController stat = FindObjectOfType<GameStatisticController>();
        if (stat != null)
            stat.buttonEvent("Загружена карта " + mapData.mapname);

        if (int.TryParse(mapData.skybox, out int skyboxValue))
            sp.skybox = skyboxValue;
        else
            Debug.LogError("Не удалось преобразовать skybox в число.");

        sp.dieCord = string.IsNullOrEmpty(mapData.diecord) ? "0,2,0" : mapData.diecord;
        sp.hp = mapData.uron;
        sp.spawnn = mapData.canSpawnObj;
        sp.destroy = mapData.canDellObj;

        StrahLoad(mapData);
    }

    public virtual void StrahLoad(MapData mapData)
    {
        if (mapData == null || mapData.objects == null)
            return;

        foreach (MapObject mapObject in mapData.objects)
        {
            GameObject prefab = Resources.Load<GameObject>(mapObject.folderLocation);
            Debug.Log(mapObject.folderLocation);

            if (prefab == null)
                continue;

            GameObject obj = Instantiate(prefab, mapObject.position, Quaternion.Euler(mapObject.rotation));
            obj.transform.localScale = mapObject.scale;

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                NetworkServer.Spawn(obj);
            }
            else
            {
                NetworkIdentity ni = obj.GetComponent<NetworkIdentity>();
                if (ni != null) Destroy(ni);

                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                if (slider != null && slider.Length > 0 && slider[0] != null)
                    obj.transform.parent = slider[0].transform;
            }

            name24 name24Comp = obj.GetComponent<name24>();
            scriptor scriptorComp = obj.GetComponent<scriptor>();

            if (name24Comp == null || scriptorComp == null)
                continue;

            name24Comp.sugoma224 = mapObject.color;
            name24Comp.isLomatel = mapObject.isLomatel;

            try
            {
                if (serverProperties.instance != null)
                    serverProperties.instance.allBlocks.Add(name24Comp);
            }
            catch
            {
            }

            if (mapObject.isObject)
                obj.tag = "object";

            if (!string.IsNullOrEmpty(mapObject.texture))
            {
                if (mapObject.texture.Contains("eggodetexture//") && mapData.textures != null)
                {
                    foreach (TextureData item in mapData.textures)
                    {
                        if (mapObject.texture != "eggodetexture//" + item.nameoftexture)
                            continue;

                        if (item.bytes != null)
                        {
                            foreach (byte b in item.bytes)
                                name24Comp.bytesForTexture.Add(b);
                        }

                        name24Comp.texture = mapObject.texture;
                        break;
                    }
                }
                else
                {
                    name24Comp.texture = mapObject.texture;
                }
            }

            if (!string.IsNullOrEmpty(mapObject.textureTile))
                name24Comp.textureTile = mapObject.textureTile;

            if (mapObject.isCollider)
                name24Comp.isCollider = mapObject.isCollider;

            if (mapObject.isRigidbody)
            {
                name24Comp.isRigidbody = mapObject.isRigidbody;
            }
            else
            {
                NetworkRigidbodyReliable nrr = obj.GetComponent<NetworkRigidbodyReliable>();
                if (nrr != null) Destroy(nrr);

                NetworkRigidbodyUnreliable nru = obj.GetComponent<NetworkRigidbodyUnreliable>();
                if (nru != null) Destroy(nru);
            }

            scriptorComp.type = mapObject.type;

            if (!string.IsNullOrEmpty(mapObject.TpCord))
                scriptorComp.TpCord = mapObject.TpCord;

            if (!string.IsNullOrEmpty(mapObject.id))
                name24Comp.id = mapObject.id;

            if (!string.IsNullOrEmpty(mapObject.Animation))
                scriptorComp.Animation = mapObject.Animation;

            if (!string.IsNullOrEmpty(mapObject.Destroy))
                scriptorComp.Destroy = mapObject.Destroy;

            if (!string.IsNullOrEmpty(mapObject.Damagenum))
                scriptorComp.Damagenum = mapObject.Damagenum;

            if (!string.IsNullOrEmpty(mapObject.Speed))
                scriptorComp.Speed = mapObject.Speed;

            if (!string.IsNullOrEmpty(mapObject.SetSize))
                scriptorComp.SetSize = mapObject.SetSize;

            if (!string.IsNullOrEmpty(mapObject.Jump))
                scriptorComp.Jump = mapObject.Jump;

            if (!string.IsNullOrEmpty(mapObject.PlayAnim))
                scriptorComp.PlayAnim = mapObject.PlayAnim;

            if (mapObject.II)
                scriptorComp.II = mapObject.II;

            scriptorComp.SetPlayerVarible = mapObject.SetPlayerVarible;
            scriptorComp.SetIntPlayerVarible = mapObject.SetIntPlayerVarible;
            scriptorComp.PlayerVaribleIf = mapObject.PlayerVaribleIf;
            scriptorComp.PlayerVaribleIfMoreInt = mapObject.PlayerVaribleIfMoreInt;
            scriptorComp.AddItem = mapObject.AddItem;
            scriptorComp.nodesCode = mapObject.node;
        }
    }
}
