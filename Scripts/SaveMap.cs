using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveMap : MonoBehaviour
{
    public string mapName;
    public string author;
    public List<GameObject> objects;
    public GameObject docher;

    private void Start()
    {
        AddChildrenOfDocherToObjects();
        SaveMapData();
    }

    private void AddChildrenOfDocherToObjects()
    {
        foreach (Transform child in docher.transform)
        {
            objects.Add(child.gameObject);
        }
    }

    public void SaveMapData()
    {
        // Create a new JSON object to store the map data
        MapData mapData = new MapData();
        mapData.mapname = mapName;
        mapData.author = author;

        // Create a list of object data to store the data for each object in the map
        List<ObjectData> objectData = new List<ObjectData>();

        // Loop through each object in the map and add its data to the list
        foreach (GameObject obj in objects)
        {
            ObjectData data = new ObjectData();
            data.folderLocation = "mapcreator/" + obj.GetComponent<name24>().name244; 
            data.position = obj.transform.position;
            data.rotation = obj.transform.rotation;
            data.scale = obj.transform.localScale;

            objectData.Add(data);
        }

        // Add the list of object data to the map data object
        mapData.objects = objectData;

        // Convert the map data object to a JSON string
        string json = JsonUtility.ToJson(mapData);

        // Save the JSON string to a file
        File.WriteAllText(Application.dataPath + "/maps/" + mapName + ".eggodemap", json);
    }

    [System.Serializable]
    public class MapData
    {
        public string mapname;
        public string author;
        public List<ObjectData> objects;
    }

    [System.Serializable]
    public class ObjectData
    {
        public string folderLocation;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}