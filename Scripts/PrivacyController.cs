using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyController : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("eula"))
        {
            gameObject.SetActive(false);
        }
    }

    public void SCARYMAY()
    {
            PlayerPrefs.SetString("eula", "true");
            PlayerPrefs.Save();
            gameObject.SetActive(false);
    }
}
