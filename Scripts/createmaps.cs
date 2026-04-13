using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mirror;

public class createmaps : MonoBehaviour
{
    public string[] path;
    public GameObject prefabik;
    public GameObject slider;

    void Start()
    {
        path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "maps"), "*.eggodemap", SearchOption.AllDirectories);
        print(path);
        create();
    }

    public void create()
    {
        foreach (string path in path)
        {
            GameObject obj = Instantiate(prefabik, new Vector3(0,0,0), Quaternion.identity);
            obj.transform.parent = slider.transform;
            obj.GetComponent<mapdannie>().jsonFilePath = path;
            obj.GetComponent<mapdannie>().Starting();
        }
    }
}
