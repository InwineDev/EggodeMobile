using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ItemLoaderLobby : MonoBehaviour
{
    [SerializeField] private string[] path;
    public List<GameObject> objects = new List<GameObject>();
    private static bool strah;

    void Start()
    {

        print(strah);
        if (strah == false)
        {
            path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "customItems"), "*.eggodeitem", SearchOption.AllDirectories);
            strah = true;
            StartCoroutine(loadBundle(path));
        }
        else
        {
            path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "customItems"), "*.eggodeitem", SearchOption.AllDirectories);
            StartCoroutine(loadBundle(path));
        }
    }

    IEnumerator loadBundle(string[] path)
    {
        AssetBundle.UnloadAllAssetBundles(true);
        foreach (var i in path)
        {

            while (!Caching.ready) yield return null;

            var www = new WWW(i);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                print(www.error);
                yield break;
            }

            var assetBan = www.assetBundle;

            GameObject[] loadedObject = assetBan.LoadAllAssets<GameObject>();

            foreach (var item in loadedObject)
            {
                objects.Add(item);
                FindObjectOfType<NetworkManager>().spawnPrefabs.Add(item);
                var sus = Instantiate(item);
            }
        }
    }
}
