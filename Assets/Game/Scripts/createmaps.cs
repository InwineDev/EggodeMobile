using System.Collections.Generic;
using UnityEngine;

public class createmaps : MonoBehaviour
{
    public GameObject prefabik;
    public GameObject slider;

    private void Start()
    {
        CreateFromPersistentStorage();
        CreateFromResources();
    }

    private void CreateFromPersistentStorage()
    {
        foreach (string path in UserContentPaths.EnumeratePersistentMapFiles())
        {
            CreateCard(path, null, null);
        }
    }

    private void CreateFromResources()
    {
        foreach (TextAsset mapJson in UserContentPaths.LoadBuiltInMapAssets())
        {
            if (mapJson == null)
                continue;

            string resourcePath = UserContentPaths.ResolveResourceMapPath(mapJson.name);
            CreateCard(null, resourcePath, mapJson.text);
        }
    }

    private void CreateCard(string persistentPath, string resourcePath, string embeddedJson)
    {
        GameObject obj = Instantiate(prefabik, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(slider.transform, false);

        mapdannie card = obj.GetComponent<mapdannie>();
        card.jsonFilePath = persistentPath;
        card.resourceMapPath = resourcePath;
        card.embeddedJson = embeddedJson;
        card.Starting();
    }
}
