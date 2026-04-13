using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class mapzagruzka : MonoBehaviour
{
    public List<string> texts = new List<string>();
    [SerializeField] private TMP_Text textTMP;

    void Start()
    {
        Invoke("LoadingEnd", Random.Range(1f, 5f));
        textTMP.text = "Совет: " + texts[Random.Range(0, texts.Count)];
    }

    void LoadingEnd()
    {
        gameObject.SetActive(false);
    }

}
