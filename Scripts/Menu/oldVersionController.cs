using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class oldVersionController : MonoBehaviour
{
    [SerializeField] private string version;
    [SerializeField] private GameObject versionOld;
    [SerializeField] private GameObject versionBeta;
    [SerializeField] private bool isBetaTest;

    void Awake()
    {
        StartCoroutine(GetJsonFromAddress());
    }

    private IEnumerator GetJsonFromAddress()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.epicsusgames.ru/eggode2/getVersion");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            print(json);
            versionGet data = JsonUtility.FromJson<versionGet>(json);
            version = data.version;
            if (version != menuManager.publicVersion & !isBetaTest)
            {
                versionOld.SetActive(true);
            }
            /*if (isBetaTest)
            {
                versionBeta.SetActive(true);
            }*/
        }
    }
}
[System.Serializable]
public class versionGet
{
    public string version;
}