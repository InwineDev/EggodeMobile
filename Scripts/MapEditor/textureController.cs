using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class textureController : MonoBehaviour
{
    public byte[] textureBytes;
    public int width;
    public int height;
    public string myname;
    [SerializeField] private RawImage preview;
    [SerializeField] private TMP_InputField nameOfTexture;
    
    public void DestroyMe()
    {
        transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<textureList>().DestroyTexture(gameObject.GetComponent<textureController>());
    }

    public void OnInputChange(string inp)
    {
        myname = inp;
    }
    public void ChooseImage()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    public void reloadInfo()
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        tex.LoadImage(textureBytes);
        tex.Apply();

        preview.texture = tex;
        nameOfTexture.text = myname;
    }
    private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);
        yield return loader;

        // Получаем оригинальные размеры текстуры
        int originalWidth = loader.texture.width;
        int originalHeight = loader.texture.height;

        // Вычисляем новые размеры с сохранением пропорций
        int newWidth = 256;
        int newHeight = Mathf.RoundToInt((float)originalHeight / originalWidth * newWidth);

        // Создаем временную текстуру для сжатия
        Texture2D compressedTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

        // Масштабируем текстуру
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(loader.texture, rt);
        RenderTexture.active = rt;
        compressedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        compressedTexture.Apply();
        RenderTexture.ReleaseTemporary(rt);

        // Сохраняем новые данные
        width = newWidth;
        height = newHeight;
        textureBytes = compressedTexture.EncodeToPNG(); // или другой формат по вашему выбору
        preview.texture = compressedTexture;

        // Очищаем оригинальную текстуру
        loader.Dispose();
    }
}
