using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class mapdannie : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text authorText;
    public string jsonFilePath;
    public string resourceMapPath;
    [TextArea] public string embeddedJson;
    public NetworkManager omg;
    public Image icon;
    public Sprite sprite;
    public Image background;
    public Image backgroundMema;
    public GameObject hostButton;
    public GameObject ErrorText;

    [SerializeField] private Color averageColor;

    public void Starting()
    {
        ReadAndApplyMapInfo();
    }

    public void ButtonGame()
    {
        login.urlMap = jsonFilePath;
        SelectedMapState.PersistentMapPath = jsonFilePath;
        SelectedMapState.ResourcesMapPath = resourceMapPath;
        SelectedMapState.EmbeddedMapJson = embeddedJson;

        hostSettings.onChangeMap?.Invoke(icon.sprite, nameText.text, authorText.text, averageColor);
    }

    private void OnEnable()
    {
        omg = FindObjectOfType<NetworkManager>();

        if (hostButton != null)
            hostButton.SetActive(true);

        if (ErrorText != null)
            ErrorText.SetActive(false);

        ReadAndApplyMapInfo();
    }

    private void ReadAndApplyMapInfo()
    {
        string jsonText = null;

        if (!string.IsNullOrEmpty(embeddedJson))
        {
            jsonText = embeddedJson;
        }
        else if (!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
        {
            jsonText = File.ReadAllText(jsonFilePath);
        }
        else if (!string.IsNullOrEmpty(resourceMapPath))
        {
            TextAsset mapAsset = Resources.Load<TextAsset>(resourceMapPath);
            if (mapAsset != null)
                jsonText = mapAsset.text;
        }

        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogWarning($"Map json not found. File='{jsonFilePath}', Resource='{resourceMapPath}'");
            return;
        }

        MapData mapData1 = JsonUtility.FromJson<MapData>(jsonText);
        if (mapData1 == null)
            return;

        nameText.text = mapData1.mapname;
        authorText.text = "Автор: " + mapData1.author;
        StartCoroutine(LoadPreview(mapData1.icon));
    }

    private IEnumerator LoadPreview(string iconValue)
    {
        if (string.IsNullOrWhiteSpace(iconValue))
            yield break;

        if (UserContentPaths.LooksLikeWebUrl(iconValue))
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(iconValue))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                    ApplyTexture(DownloadHandlerTexture.GetContent(www));
                else
                    Debug.LogWarning("Error loading image: " + www.error);
            }
            yield break;
        }

        if (UserContentPaths.LooksLikeAbsoluteFilePath(iconValue))
        {
            Texture2D fileTexture = UserContentPaths.LoadTextureFromAbsolutePath(iconValue);
            if (fileTexture != null)
                ApplyTexture(fileTexture);
            yield break;
        }

        Sprite resourceSprite = Resources.Load<Sprite>(iconValue);
        if (resourceSprite != null)
        {
            ApplySprite(resourceSprite);
            yield break;
        }

        Texture2D resourceTexture = Resources.Load<Texture2D>(iconValue);
        if (resourceTexture != null)
            ApplyTexture(resourceTexture);
    }

    private void ApplyTexture(Texture2D texture)
    {
        if (texture == null)
            return;

        Sprite createdSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        ApplySprite(createdSprite);
    }

    private void ApplySprite(Sprite loadedSprite)
    {
        sprite = loadedSprite;
        icon.sprite = sprite;
        background.sprite = sprite;
        averageColor = GetAverageColor(sprite.texture);
        backgroundMema.color = averageColor;
    }

    private Color GetAverageColor(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        if (pixels == null || pixels.Length == 0)
            return Color.white;

        long totalR = 0;
        long totalG = 0;
        long totalB = 0;
        long totalA = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            totalR += pixels[i].r;
            totalG += pixels[i].g;
            totalB += pixels[i].b;
            totalA += pixels[i].a;
        }

        return new Color(
            (float)totalR / pixels.Length / 255f,
            (float)totalG / pixels.Length / 255f,
            (float)totalB / pixels.Length / 255f,
            (float)totalA / pixels.Length / 255f);
    }
}
