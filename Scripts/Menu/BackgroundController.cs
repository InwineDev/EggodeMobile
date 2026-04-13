using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public static Action<int> ChangeBG;

    private void OnEnable()
    {
        ChangeBG += ChangeBg;
    }

    private void OnDisable()
    {
        ChangeBG -= ChangeBg;
    }

    [SerializeField] private List<GameObject> bgList = new List<GameObject>();
    private void ChangeBg(int bg)
    {
        foreach (var item1 in bgList)
        {
            item1.SetActive(false);
        }
        bgList[bg].SetActive(true);
    }
}
