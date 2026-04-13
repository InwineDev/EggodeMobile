using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string scenes;
    public bool onStart;
    public float seconds;

    private void Start()
    {
        if (onStart)
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(scenes);
        }
    }

    public void Starting()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        if (scenes == "MapEdit")
        {
            yield return new WaitForSeconds(seconds);

            NetworkManager manager = FindObjectOfType<NetworkManager>();
            if (manager != null)
            {
                manager.StartHost();
                manager.ServerChangeScene("MapEdit");
            }

            SceneManager.LoadScene(scenes);
            yield break;
        }

        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(scenes);
    }
}