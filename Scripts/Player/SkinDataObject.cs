using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinDataObject : MonoBehaviour
{
    public int id;
    public SkinSaver skinSaver;
    public string type;
    public Image preview;

    public void ChangeSkin()
    {
        skinSaver.changeSkin?.Invoke(id, type);
    }
}
