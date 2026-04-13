using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogeSettings
{
    [TextArea()]
    public string dialogeText;
    public string dialogeAuthor;
    public Sprite dialogeSprite;
    public float dialogeTime;
}
public class DialogeTrigger : MonoBehaviour
{
    [Header("Dialoge Settings")]
    public List<DialogeSettings> dialogeSettings = new List<DialogeSettings>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogeController.showDialoge?.Invoke(dialogeSettings);
            Destroy(this);
        }
    }
}