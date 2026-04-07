using System;
using System.Linq;
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
        uiToChangeImage.sprite = sp;
        uiToChangeImageBackground.sprite = sp;

        Color color1 = color;
        color1.a = 1f;
        uiToChangeImageBackgroundUp.color = color1;

        nameText.text = name;
        authorText.text = author;
    }

    public void host()
    {
        if (string.IsNullOrEmpty(login.urlMap) && string.IsNullOrEmpty(SelectedMapState.ResourcesMapPath))
        {
            string randomPersistentMap = UserContentPaths.EnumeratePersistentMapFiles().OrderBy(_ => UnityEngine.Random.value).FirstOrDefault();
            if (!string.IsNullOrEmpty(randomPersistentMap))
            {
                login.urlMap = randomPersistentMap;
                SelectedMapState.PersistentMapPath = randomPersistentMap;
            }
            else
            {
                TextAsset[] validMaps = UserContentPaths.LoadBuiltInMapAssets()
                    .Where(t => t != null && !string.IsNullOrWhiteSpace(t.text) && t.text.TrimStart().StartsWith("{") && t.text.Contains("\"mapname\""))
                    .ToArray();

                if (validMaps.Length > 0)
                {
                    TextAsset selected = validMaps[UnityEngine.Random.Range(0, validMaps.Length)];
                    SelectedMapState.ResourcesMapPath = UserContentPaths.ResolveResourceMapPath(selected.name);
                    SelectedMapState.EmbeddedMapJson = selected.text;
                }
                else
                {
                    Debug.LogError("No valid built-in map JSON files found in Resources/Maps or Resources/maps.");
                }
            }
        }

        MultiplayerManager.instance.CreateLobby(0, int.Parse(playersValue.text));
    }
}
