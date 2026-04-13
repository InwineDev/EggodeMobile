using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hostSettings : MonoBehaviour
{
    [SerializeField] private Image uiToChangeImage;
    [SerializeField] private Image uiToChangeImageBackground;
    [SerializeField] private Image uiToChangeImageBackgroundUp;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text authorText;
    [SerializeField] private TMP_Dropdown onlineMode;
    [SerializeField] private TMP_InputField playersValue;

    public static Action<Sprite, string, string, Color> onChangeMap;

    private void OnEnable()
    {
        onChangeMap += Change;
    }

    private void OnDisable()
    {
        onChangeMap -= Change;
    }

    public void Change(Sprite sp, string name, string author, Color color)
    {
        if (uiToChangeImage != null)
            uiToChangeImage.sprite = sp;

        if (uiToChangeImageBackground != null)
            uiToChangeImageBackground.sprite = sp;

        Color color1 = color;
        color1.a = 1f;

        if (uiToChangeImageBackgroundUp != null)
            uiToChangeImageBackgroundUp.color = color1;

        if (nameText != null)
            nameText.text = name;

        if (authorText != null)
            authorText.text = author;
    }

    public void host()
    {
        Debug.Log("Players value: " + playersValue.text);

        if (string.IsNullOrEmpty(login.urlMap))
        {
            string mapsFolder = Path.Combine(Path.GetDirectoryName(Application.dataPath), "maps");

            if (!Directory.Exists(mapsFolder))
            {
                Debug.LogError("Maps folder not found: " + mapsFolder);
                return;
            }

            string[] paths = Directory.GetFiles(
                mapsFolder,
                "*.eggodemap",
                SearchOption.AllDirectories
            );

            if (paths.Length == 0)
            {
                Debug.LogError("No .eggodemap files found in: " + mapsFolder);
                return;
            }

            login.urlMap = paths[UnityEngine.Random.Range(0, paths.Length)];
        }

        int maxPlayers = 4;
        if (!int.TryParse(playersValue.text, out maxPlayers))
        {
            maxPlayers = 4;
        }

        MultiplayerManager.instance.CreateLobby(maxPlayers);
    }
    
}