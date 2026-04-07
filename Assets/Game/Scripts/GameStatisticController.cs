using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameStatisticController : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        StartCoroutine(GetJsonFromAddress("Игрок вышел из игры"));
    }

    public IEnumerator GetJsonFromAddress(string message)
    {
        if (settingsController.sborDannie)
        {
            string data =
                $"{message}\n" +
                $"Login:{login.username}\n" +
                $"GameSettings:\n{settingsController.jsonSettings}\n" +
                $"Version:{menuManager.publicVersion}";

            string encodedData = Uri.EscapeDataString(data);
            string url = $"https://www.epicsusgames.ru/eggode2/data/sendinfo.php?moreinfo={encodedData}";

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Successfully sent data!");
            }
        }
    }

    public void buttonEvent(string what)
    {
        StartCoroutine(GetJsonFromAddress(what));
    }

    private void Start()
    {
        StartCoroutine(GetJsonFromAddress("Игрок зашёл в игру"));
    }
}