using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR) && !UNITY_ANDROID
using SFB;
#endif

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
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR) && !UNITY_ANDROID
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths != null && paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            StartCoroutine(OutputRoutine(paths[0]));
        }
#else
        Debug.LogWarning("ChooseImage is not supported on Android in this build. Hook it up to a mobile file picker plugin.");
#endif
    }

    public void reloadInfo()
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.LoadImage(textureBytes);
        tex.Apply();

        preview.texture = tex;
        nameOfTexture.text = myname;
    }

    private IEnumerator OutputRoutine(string path)
    {
        string uri = path;
        if (!uri.StartsWith("file://"))
            uri = new System.Uri(path).AbsoluteUri;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogWarning("Failed to load texture: " + request.error);
                yield break;
            }

            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(request);
            if (loadedTexture == null)
                yield break;

            int originalWidth = loadedTexture.width;
            int originalHeight = loadedTexture.height;
            int newWidth = 256;
            int newHeight = Mathf.Max(1, Mathf.RoundToInt((float)originalHeight / Mathf.Max(1, originalWidth) * newWidth));

            Texture2D compressedTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            Graphics.Blit(loadedTexture, rt);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            compressedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            compressedTexture.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);

            width = newWidth;
            height = newHeight;
            textureBytes = compressedTexture.EncodeToPNG();
            preview.texture = compressedTexture;
        }
    }
}
