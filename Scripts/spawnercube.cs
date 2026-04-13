using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class spawnercube : MonoBehaviour
{
    public List<GameObject> cubessssss = new List<GameObject>();
    public List<string> cubessssssS = new List<string>();
    [SerializeField] private int sugoma = 0;
    [SerializeField] private GameObject slider;
    [SerializeField] private string[] path;
    private string strah1;
    [SerializeField] private GameObject prefabData;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject editorItems;

    [SerializeField] private TMP_Text nameSelected;

    private void Start()
    {
        //path = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "editorAssets/"), "*.eggodeeditor", SearchOption.AllDirectories);
        //strah1 = Path.Combine(Path.GetDirectoryName(Application.dataPath), "editorAssets/");

        loadBundles();
    }
    public void sugomamami(int newsugoma)
    {
        sugoma = newsugoma;
        nameSelected.text = cubessssss[newsugoma].name;
        editorItems.SetActive(false);
    }

    public void SpawnObject()
    {
        GameObject subebra = Instantiate(cubessssss[sugoma], new Vector3(Mathf.Round(Camera.main.transform.position.x), Mathf.Round(Camera.main.transform.position.y), Mathf.Round(Camera.main.transform.position.z)), Quaternion.identity);
        subebra.transform.parent = slider.transform;
        subebra.SetActive(true);
        Destroy(subebra.GetComponent<NetworkIdentity>());
    }
    void loadBundles()
    {
        try
        {
            foreach (var i in FindObjectOfType<ModLoader>().objectsMapEditor)
            {
                cubessssss.Add(i);
            }
            foreach (var i in cubessssss)
            {
                cubessssssS.Add(i.name);
            }
        }
        catch
        {

        }
        for (int i = 0; i < cubessssss.Count; i++)
        {
            AddContent(i);
        }
    }

    private void AddContent(int id)
    {
        GameObject localObj = Instantiate(prefabData, content);

        localObj.GetComponent<EditorItemController>().init(id, this);
    }
}
