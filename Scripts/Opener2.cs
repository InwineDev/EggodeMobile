using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static userSettingNotCam;

public class Opener2 : MonoBehaviour
{
    public GameObject[] open;


    public void openclass(int sus1)
    {
        foreach (GameObject sus in open)
        {
            sus.SetActive(false);
        }

        open[sus1].SetActive(true);
    }
}
