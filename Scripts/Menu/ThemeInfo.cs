using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThemeInfo : MonoBehaviour
{

    public int themeId;
    public string themeName;

    [SerializeField] private TMP_Text themeText; 
    private void OnEnable()
    {
        themeText.text = themeName;
    }
    public void ChangeTheme()
    {
        settingsController.themeChange?.Invoke(themeId);
    }
}
