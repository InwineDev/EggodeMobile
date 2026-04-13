using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvents : MonoBehaviour
{
    public userSettings us;
    [Header("Dialoge Settings")]
    public List<DialogeSettings> dialogeSettingsStol = new List<DialogeSettings>();
    private bool stolDialoge;

    private void Start()
    {
        Invoke("Subscribe", 3f);
    }
    private void OnEnable()
    {
        us = FindObjectOfType<userSettings>();
        us.OnChangeItem += Dialoge;
    }

    void Subscribe()
    {
        us = FindObjectOfType<userSettings>();
        us.OnChangeItem += Dialoge;
    }

    private void OnDisable()
    {
        us.OnChangeItem -= Dialoge;
    }

    void Dialoge(int id)
    {
        if(id == 1 & !stolDialoge)
        {
            DialogeController.showDialoge?.Invoke(dialogeSettingsStol);
            stolDialoge = true;
        }
    }
}
