using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditroTabs : MonoBehaviour
{
    public List<GameObject> tabs = new List<GameObject>();
    
    public void changeTab(int strah)
    {
        foreach (var item in tabs)
        {
            item.transform.localPosition = new Vector3(0, 1500, 0);
        }
        tabs[strah].transform.localPosition = new Vector3(0, 0, 0);
    }
}
