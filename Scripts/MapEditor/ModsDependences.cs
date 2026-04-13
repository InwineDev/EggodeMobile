using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModsDependences : MonoBehaviour
{
    public GameObject dependencesPrefab;
    public GameObject content;
    public List<DependenceController> dcs = new List<DependenceController>();

    public void CreateDependences()
    {
        AddDependences("");
    }

    public void AddDependences(string path)
    {
        GameObject dc = Instantiate(dependencesPrefab);
        dc.transform.SetParent(content.transform);
        DependenceController dcc = dc.GetComponent<DependenceController>();
        dcs.Add(dcc);
        dcc.mD = this;
        dcc.tmpif.text = path;
    }
}
