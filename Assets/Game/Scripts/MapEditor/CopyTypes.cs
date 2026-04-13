using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyTypes : MonoBehaviour
{
    public List<TMP_InputField> inputs = new List<TMP_InputField>();

    public void Copy()
    {
        string tocopy = "";
        foreach (var item in inputs)
        {
            tocopy += item.text + ".";
        }
        int x1 = tocopy.Length - 1;
        tocopy = tocopy.Remove(x1);
        GUIUtility.systemCopyBuffer = tocopy;
    }

    public void Paste()
    {
        string topaste = GUIUtility.systemCopyBuffer;
        string[] elements = topaste.Split('.');
        for (int i = 0; i < inputs.Count; i++)
        {
            try
            {
                inputs[i].text = elements[i];
            }
            catch
            {
                print("MEME");
            }
        }
    }
}