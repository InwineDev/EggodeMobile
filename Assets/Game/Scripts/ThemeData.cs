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

    public void ChangeTheme(int themeIndex)
    {
        if (themeIndex >= 0 && themeIndex < allThemes.Count)
        {
            selectedTheme = allThemes[themeIndex];
        }
    }

    private void Awake()
    {
        me = this;
        Load();
    }

    public void Load()
    {
        allThemes.Clear();

        string themesDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "GameConfigs", "Themes");

        if (!Directory.Exists(themesDir))
        {
            Debug.LogWarning($"Папка тем не найдена: {themesDir}");
            return;
        }

        DirectoryInfo info = new DirectoryInfo(themesDir);
        FileInfo[] fileInfo = info.GetFiles();

        foreach (var item in fileInfo)
        {
            string jsonText = File.ReadAllText(item.FullName);
            theme loadedTheme = JsonUtility.FromJson<theme>(jsonText);

            allThemes.Add(new theme(
                loadedTheme.name,
                loadedTheme.author,
                loadedTheme.textColor,
                loadedTheme.backgroundColor,
                loadedTheme.color
            ));
        }

        if (allThemes.Count > 0)
        {
            selectedTheme = allThemes[0];
        }
    }
}