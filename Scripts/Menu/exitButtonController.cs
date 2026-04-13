using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitButtonController : MonoBehaviour
{
    public GameObject catscene;

    public void ExitFromGame()
    {
        Application.Quit();
    }
    void OnEnable()
    {
        settingsController.killExitButtonAction += KillMe;
        settingsController.aliveExitButtonAction += aliveMe;
    }
    void OnDisable()
    {
        settingsController.killExitButtonAction -= KillMe;
        settingsController.aliveExitButtonAction -= aliveMe;
    }

    void KillMe()
    {
        catscene.SetActive(true);
        GetComponent<textEffect>().enabled = false;
        GetComponent<ButtonEffect>().enabled = true;
    }

    void aliveMe()
    {
        catscene.SetActive(false);
        GetComponent<textEffect>().enabled = true;
        GetComponent<ButtonEffect>().enabled = false;
    }
}
