using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accountsettings : MonoBehaviour
{
    public void SUS()
    {
        PlayerPrefs.DeleteKey("abobusisus");
        Application.Quit();
    }
}
