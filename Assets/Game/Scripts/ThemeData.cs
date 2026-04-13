using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class theme
{
    public string name;
    public string author;
    public Color textColor;
    public Color backgroundColor;
    public Color color;

    public theme(string name, string author, Color textColor, Color backgroundColor, Color color)
    {
        this.name = name;
        this.author = author;
        this.textColor = textColor;
        this.backgroundColor = backgroundColor;
        this.color = color;
    }
}

public class ThemeData : MonoBehaviour
{
    public List<theme> allThemes = new List<theme>();
    public static ThemeData me;
    public theme selectedTheme;

    private void OnEnable()
    {
        settingsController.themeChange += ChangeTheme;
    }

    private void OnDisable()
    {
        settingsController.themeChange -= ChangeTheme;
    }

    public void ChangeTheme(int theme)
    {
        selectedTheme = allThemes[theme];
    }

    void Awake()
    {
        me = this;
        load();
    }

    public void load()
    {
        var info = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Themes"));
        var fileInfo = info.GetFiles();
        foreach (var item in fileInfo)
        {
            string jsonText = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Themes", item.FullName));
            theme Theme = JsonUtility.FromJson<theme>(jsonText);
            allThemes.Add(new theme(Theme.name, Theme.author, Theme.textColor, Theme.backgroundColor, Theme.color));
        }
        selectedTheme = allThemes[0];
    }
}