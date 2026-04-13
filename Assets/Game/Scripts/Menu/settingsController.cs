using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static SaveMap;

public class settingsController : MonoBehaviour
{
    [SerializeField] private RenderPipelineAsset[] levelsettings;
    [SerializeField] private TMP_Dropdown graphic;
    [SerializeField] private TMP_Dropdown scale;
    [SerializeField] private Toggle sborDannih;
    [SerializeField] private Toggle developerMode;
    public static bool developer = false;
    private int zagruzeno = 0;
    private int maxFPS = 500;
    public Slider FPS;
    public Slider MUSIC;
    public TMP_Text FPSTXT;
    public TMP_Text MUSICTXT;
    private int QualityLevel = 0;

    public static bool sborDannie = true;
    private bool killExitButton;

    public Toggle killExitButtonToggle;

    public GameObject nastroiki;
    [SerializeField] private AudioSource musicSource;
    public static string jsonSettings;
    public static Action killExitButtonAction;
    public static Action aliveExitButtonAction;
    public int themeNumber;

    [Header("Nicknames")]
    public static string nickname;
    public TMP_InputField nickField;

    [Header("Themes")]
    public GameObject prefabTheme;
    public GameObject content;
    public List<GameObject> themes = new List<GameObject>();
    public static Action<int> themeChange;
    private int themeChoosen;
    [SerializeField] private TMP_Text choosedTheme;
    public SkinLoader playerSkin;

    [Header("Voice Chat")]
    [SerializeField] private TMP_Dropdown microphoneList;

    public static int micronum;

    private void OnEnable()
    {
        themeChange += ChangeSavedTheme;
    }

    private void OnDisable()
    {
        themeChange -= ChangeSavedTheme;
    }

    public void OnChangeNickname(string newNick)
    {
        if (newNick.Length > 10)
        {
            nickname = newNick.Substring(0, 10);
        }
        else
        {
            nickname = newNick;
        }
    }

    public void ChangeMicro(int num)
    {
        micronum = num;
    }

    private void ChangeSavedTheme(int toSave)
    {
        themeChoosen = toSave;
        choosedTheme.text = "������� ���� �� ������ " + toSave;
    }

    public void ChangeLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = levelsettings[value];
        QualityLevel = value;
    }

    private void Start()
    {
        LoadThemes();
        Load();

        zagruzeno = 2;
        if (zagruzeno == 2)
        {
            nastroiki.SetActive(false);
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("�������� �� ������! ���������� �������� � ������������� ����.");
            return;
        }

        List<string> options = new List<string>();
        foreach (string deviceName in Microphone.devices)
        {
            options.Add(deviceName);
        }
        microphoneList.AddOptions(options);
    }

    public void LoadThemes()
    {
        var info = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Themes"));
        var fileInfo = info.GetFiles();
        foreach (var item1 in themes)
        {
            Destroy(item1);
        }
        themes.Clear();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            string jsonText = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Themes", fileInfo[i].FullName));
            theme Theme = JsonUtility.FromJson<theme>(jsonText);
            GameObject s = Instantiate(prefabTheme);
            s.transform.SetParent(content.transform, false);
            ThemeInfo ti = s.GetComponent<ThemeInfo>();
            ti.themeName = Theme.name;
            ti.themeId = i;
            themes.Add(s);
        }
    }

    public void OnChangeDannie()
    {
        sborDannie = sborDannih.isOn;
    }

    public void OnChangeDeveloperMode()
    {
        developer = developerMode.isOn;
    }

    public void OnChangeKillExitBUtton()
    {
        killExitButton = killExitButtonToggle.isOn;
        if (killExitButton)
        {
            killExitButtonAction?.Invoke();
        }
        else
        {
            aliveExitButtonAction?.Invoke();
        }
    }

    public void SetFPS()
    {
        if (FPS.value == 0)
        {
            QualitySettings.vSyncCount = 1;
            maxFPS = (int)FPS.value;
            FPSTXT.text = "V-Syns";
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = (int)FPS.value;
            maxFPS = (int)FPS.value;
            FPSTXT.text = maxFPS.ToString();
        }
    }

    public void SetMUSIC()
    {
        musicSource.volume = Mathf.Round(MUSIC.value);
        MUSICTXT.text = Mathf.Round(MUSIC.value * 100).ToString();
    }

    public void Save()
    {
        SettingsDownloader settingsDownloader = new SettingsDownloader
        {
            sborDannieBool = sborDannih.isOn,
            developerModeBool = developer,
            maxFPS = maxFPS,
            musicVolume = MUSIC.value,
            graphic = QualityLevel,
            killExitButton = killExitButton,
            themeNumber = themeChoosen,
            nick = nickname,
            micro = micronum
        };
        string json = JsonUtility.ToJson(settingsDownloader, true);

        File.WriteAllText(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/") + "Settings.eggodesettings", json);
    }

    private void Load()
    {
        try
        {
            string jsonFilePath = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "GameConfigs/Settings.eggodesettings");
            jsonSettings = System.IO.File.ReadAllText(jsonFilePath);

            SettingsDownloader settingsDownloader = JsonUtility.FromJson<SettingsDownloader>(jsonSettings);
            FPS.value = settingsDownloader.maxFPS;
            SetFPS();
            sborDannie = settingsDownloader.sborDannieBool;
            sborDannih.isOn = sborDannie;
            MUSIC.value = settingsDownloader.musicVolume;
            SetMUSIC();
            graphic.value = settingsDownloader.graphic;
            themeNumber = settingsDownloader.themeNumber;
            themeChange?.Invoke(themeNumber);
            ChangeLevel(settingsDownloader.graphic);
            if (settingsDownloader.nick.Length > 10)
            {
                nickname = settingsDownloader.nick.Substring(0, 10);
            }
            else
            {
                nickname = settingsDownloader.nick;
            }
            nickField.text = nickname;
            killExitButton = settingsDownloader.killExitButton;
            killExitButtonToggle.isOn = killExitButton;
            developer = settingsDownloader.developerModeBool;
            developerMode.isOn = developer;
            if (killExitButton)
            {
                killExitButtonAction?.Invoke();
            }
            else
            {
                aliveExitButtonAction?.Invoke();
            }
            playerSkin.LocalLoad();
            micronum = settingsDownloader.micro;
            microphoneList.value = micronum;
        }
        catch
        {
        }
    }
}

[System.Serializable]
public class SettingsDownloader
{
    public bool sborDannieBool;
    public bool developerModeBool;
    public int maxFPS;
    public float musicVolume;
    public int graphic;
    public bool killExitButton;
    public int themeNumber;
    public string nick;
    public int micro;
}