using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeObjChooser : MonoBehaviour
{
    public int selectedColor = 0;
    public int typeColor = 0;

    private void Start()
    {
        changeTheme();
    }
    void OnEnable()
    {
        changeTheme();
    }

    void changeTheme()
    {
        try
        {
            if (typeColor == 0)
            {
                if (selectedColor == 0) GetComponent<Image>().color = ThemeData.me.selectedTheme.color;
                if (selectedColor == 1) GetComponent<Image>().color = ThemeData.me.selectedTheme.backgroundColor;
            }
            if (typeColor == 1)
            {
                GetComponent<TMP_Text>().color = ThemeData.me.selectedTheme.textColor;
            }
        }
        catch
        {

        }
    }
}
