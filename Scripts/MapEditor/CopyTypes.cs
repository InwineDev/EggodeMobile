using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyTypes : MonoBehaviour
{
    public List<TMP_InputField> inputs = new List<TMP_InputField>();

    public void Copy()
    {
        string toCopy = "";

        foreach (var item in inputs)
        {
            toCopy += item.text + ".";
        }

        if (toCopy.Length > 0)
        {
            toCopy = toCopy.Remove(toCopy.Length - 1);
        }

        GUIUtility.systemCopyBuffer = toCopy;
    }

    public void Paste()
    {
        string toPaste = GUIUtility.systemCopyBuffer;
        string[] elements = toPaste.Split('.');

        for (int i = 0; i < inputs.Count; i++)
        {
            if (i < elements.Length)
            {
                inputs[i].text = elements[i];
            }
        }
    }
}