using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class customItems
{
    public GameObject itemInArm;
    public GameObject spawnedItem;

    public customItems(GameObject itemInArm, GameObject spawnedItem)
    {
        this.itemInArm = itemInArm;
        this.spawnedItem = spawnedItem;
    }

}

public class ModLoader : MonoBehaviour
{
    public static ModLoader instance;
    [Header("MapEditor")]
    [SerializeField] private string[] path;
    [SerializeField] private GameObject error;
    [SerializeField] private TMP_Text strahErrora;
    public List<GameObject> objectsMapEditor = new List<GameObject>();
    private static bool strah;
    [Header("Items")]
    public List<customItems> objectsItems = new List<customItems>();

    private void Awake()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        instance = this;
        print(instance.name);
    }

    //void Start()
    //{
    //    print(strah);
    //    if (strah == false)
    //    {
    //        path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "editorAssets"), "*.eggodeeditor", SearchOption.AllDirectories);
    //        strah = true;
    //        StartCoroutine(loadBundle(path));
    //    } else
    //    {
    //        path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "editorAssets"), "*.eggodeeditor", SearchOption.AllDirectories);
    //        StartCoroutine(loadBundle(path));
    //    }
    //}

    //IEnumerator loadBundle(string[] path)
    //{
    //    AssetBundle.UnloadAllAssetBundles(true);
    //    foreach (var i in path)
    //    {

    //        while (!Caching.ready) yield return null;

    //        var www = new WWW(i);
    //        yield return www;

    //        if (!string.IsNullOrEmpty(www.error))
    //        {
    //            print(www.error);
    //            yield break;
    //        }

    //        var assetBan = www.assetBundle;

    //        GameObject loadedObject = assetBan.LoadAsset<GameObject>(Path.GetFileNameWithoutExtension(i));
    //        if(loadedObject == null)
    //        {
    //            error.SetActive(true);
    //            strahErrora.text += "\n" +i;
    //        }
    //        objects.Add(loadedObject);
    //        gameObject.GetComponent<NetworkManager>().spawnPrefabs.Add(loadedObject);
    //    }
    //}
    //

    public void addMapBundle(GameObject loadedObject)
    {
        print("MEIEOOEOWO");
        if (loadedObject == null)
        {
            Debug.LogError("Loaded object is null!");
            return;
        }

        objectsMapEditor.Add(loadedObject);
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();

        networkManager.spawnPrefabs.Add(loadedObject);

        // Clean up null entries
        for (int i = objectsMapEditor.Count - 1; i >= 0; i--)
        {
            if (objectsMapEditor[i] == null)
            {
                objectsMapEditor.RemoveAt(i);
            }
        }
    }

    public void addItemBundle(GameObject itemInArm, GameObject spawnedInArm)
    {
        print("MEIEOOEOWO");
        if (itemInArm == null)
        {
            Debug.LogError("Loaded object is null!");
            return;
        }


        objectsItems.Add(new customItems(itemInArm, spawnedInArm));
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();

        networkManager.spawnPrefabs.Add(itemInArm);

        if(spawnedInArm != null)
        {
            networkManager.spawnPrefabs.Add(spawnedInArm);
        }
        // Clean up null entries
        for (int i = objectsItems.Count - 1; i >= 0; i--)
        {
            if (objectsItems[i] == null)
            {
                objectsItems.RemoveAt(i);
            }
        }
    }

    public void networkManagerRegister(GameObject loadedObject)
    {
        print("MEIEOOEOWO");
        if (loadedObject == null)
        {
            Debug.LogError("Loaded object is null!");
            return;
        }
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();

        networkManager.spawnPrefabs.Add(loadedObject);
    }
}
