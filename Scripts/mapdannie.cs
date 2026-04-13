using Mirror;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class mapdannie : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text authorText;
    public string jsonFilePath;
    public NetworkManager omg;
    public Image icon;
    public Texture2D texture22;
    public Sprite sprite;
    public Image background;
    public Image backgroundMema;
    public GameObject hostButton;
    public GameObject ErrorText;

    [SerializeField] private Color averageColor;

    public void Starting()
    {
    }

    public void ButtonGame()
    {
        login.urlMap = jsonFilePath;
        hostSettings.onChangeMap?.Invoke(icon.sprite, nameText.text, authorText.text, averageColor);
    }

    public IEnumerator Pon(string textureUrl)
    {
        if (string.IsNullOrWhiteSpace(textureUrl))
            yield break;

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning("Error loading image: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                if (texture != null)
                {
                    sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    if (icon != null)
                        icon.sprite = sprite;

                    if (background != null)
                        background.sprite = sprite;

                    averageColor = GetAverageColor(sprite.texture);

                    if (backgroundMema != null)
                        backgroundMema.color = averageColor;
                }
            }
        }
    }

    private Color GetAverageColor(Texture2D texture)
    {
        if (texture == null)
            return Color.white;

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

        float averageR = (float)totalR / pixels.Length / 255f;
        float averageG = (float)totalG / pixels.Length / 255f;
        float averageB = (float)totalB / pixels.Length / 255f;
        float averageA = (float)totalA / pixels.Length / 255f;

        return new Color(averageR, averageG, averageB, averageA);
    }

    private void OnEnable()
    {
        omg = FindObjectOfType<NetworkManager>();

        if (hostButton != null)
            hostButton.SetActive(false);

        if (ErrorText != null)
            ErrorText.SetActive(false);

        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            Debug.LogWarning("jsonFilePath is empty.");
            return;
        }

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogWarning("Map file not found: " + jsonFilePath);
            return;
        }

        string jsonText = File.ReadAllText(jsonFilePath);
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            Debug.LogWarning("Map file is empty: " + jsonFilePath);
            return;
        }

        MapData mapData1 = JsonUtility.FromJson<MapData>(jsonText);
        if (mapData1 == null)
        {
            Debug.LogWarning("Failed to parse map json: " + jsonFilePath);
            return;
        }

        if (nameText != null)
            nameText.text = mapData1.mapname;

        if (authorText != null)
            authorText.text = "Автор: " + mapData1.author;

        bool modsOk = true;

        if (mapData1.modsDependence != null)
        {
            modsOk = mapData1.modsDependence.All(mod => CsModsManager.modsForServer.Contains(mod));
        }

        if (modsOk)
        {
            if (hostButton != null)
                hostButton.SetActive(true);

            if (ErrorText != null)
                ErrorText.SetActive(false);
        }
        else
        {
            if (hostButton != null)
                hostButton.SetActive(false);

            if (ErrorText != null)
                ErrorText.SetActive(true);
        }

        StartCoroutine(Pon(mapData1.icon));
    }
}