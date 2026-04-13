using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReeditorController : NetworkBehaviour, interactable
{
    [Header("Exporter")]
    [SerializeField] private Transform exitPosition;
    [SerializeField] private GameObject spawnPrefab;

    [Header("dataObjects")]
    [SerializeField] private List<int> dataObjects = new List<int>();

    private void OnCollisionEnter(Collision collision)
    {
        name24 obj = collision.gameObject.GetComponent<name24>();
        if (obj)
        {
            if (obj.tag != "object") return;
            AddData(obj.name244);
            NetworkServer.Destroy(obj.gameObject);
        }
    }
    int ConvertLettersToNumbers(string input)
    {
        string result = "";
        foreach (char c in input.ToUpper())
        {
            if (char.IsLetter(c))
            {
                int num = c - 'A' + 1;
                result += num.ToString() + " ";
            }
        }
        return int.Parse(result);
    }

    [Command]
    void AddData(string name)
    {
        dataObjects.Add(ConvertLettersToNumbers(name));
    }

    private void GenerateNewObject()
    {
        CmdGenerate();
    }
    public GeneratedObjectController ob;
    [Command]
    void CmdGenerate()
    {
        GameObject newObject = Instantiate(spawnPrefab, exitPosition.position, exitPosition.rotation);
        NetworkServer.Spawn(newObject);
        ob = newObject.GetComponent<GeneratedObjectController>();
        newObject.GetComponent<GeneratedObjectController>().textureSeed = dataObjects[Random.Range(0, dataObjects.Count)];
        //RpcGenerate(obj);
    }

/*    [ClientRpc]
    void RpcGenerate(GameObject obj)
    {
        obj.GetComponent<GeneratedObjectController>().texture = dataObjects[Random.Range(0, dataObjects.Count)].texture;
    }*/
    public void interact(FirstPersonController player)
    {
        GenerateNewObject();
    }
}
