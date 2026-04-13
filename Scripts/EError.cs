using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EError : MonoBehaviour
{
    public static string error;
    [SerializeField] private GameObject errorPr;
    [SerializeField] private TMP_Text errorTxt;
    
    void Start()
    {
        if(error != null)
        {
            errorPr.SetActive(true);
            errorTxt.text = error;
        }
    }
}
