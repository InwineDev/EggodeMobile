using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemLoaderLobby : MonoBehaviour
{
    [SerializeField] private string[] path;
    public List<GameObject> objects = new List<GameObject>();
    public bool IsLoaded { get; private set; }
    private static bool strah;

    private void Start()
    {
        IsLoaded = false;
        Debug.Log(strah);

        string customItemsDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "customItems");

        if (!Directory.Exists(customItemsDir))
        {
            Debug.LogWarning($"Папка не найдена: {customItemsDir}");
            IsLoaded = true;
            return;
        }

        path = Directory.GetFiles(customItemsDir, "*.eggodeitem", SearchOption.AllDirectories);

        if (!strah)
        {
            strah = true;
        }

        StartCoroutine(LoadBundle(path));
    }

    private IEnumerator LoadBundle(string[] paths)
    {
        AssetBundle.UnloadAllAssetBundles(true);

        foreach (string bundlePath in paths)
        {
            if (string.IsNullOrEmpty(bundlePath))
                continue;

            if (!File.Exists(bundlePath))
            {
                Debug.LogWarning($"Файл не найден: {bundlePath}");
                continue;
            }

            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundleRequest;

            AssetBundle assetBundle = bundleRequest.assetBundle;

            if (assetBundle == null)
            {
                Debug.LogError($"Не удалось загрузить AssetBundle: {bundlePath}");
                continue;
            }

            AssetBundleRequest assetsRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
            yield return assetsRequest;

            Object[] loadedAssets = assetsRequest.allAssets;

            if (loadedAssets == null || loadedAssets.Length == 0)
            {
                Debug.LogWarning($"В бандле нет GameObject: {bundlePath}");
                assetBundle.Unload(false);
                continue;
            }

            NetworkManager networkManager = FindObjectOfType<NetworkManager>();

            foreach (Object loadedAsset in loadedAssets)
            {
                GameObject item = loadedAsset as GameObject;
                if (item == null)
                    continue;

                if (!objects.Contains(item))
                {
                    objects.Add(item);
                }

                if (networkManager != null && !networkManager.spawnPrefabs.Contains(item))
                {
                    networkManager.spawnPrefabs.Add(item);
                }
            }

            assetBundle.Unload(false);
        }

        IsLoaded = true;
    }
}