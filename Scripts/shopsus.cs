using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class shopsus : MonoBehaviour
{
    public string address;
    public string address2;
    public string password;
    public long suscoins = -1;
    public string skinu;
    public TMP_Text suscoinsis;
    public skinmanager skinmanager228;
    public TMP_Text debug;

    public static int omg;
    public static GameObject me;

    private void Start()
    {
        me = gameObject;
        skinloaderfromshop[] skinLoaders = FindObjectsOfType<skinloaderfromshop>();

        Itemloaderfromshop sus1234 = FindObjectOfType<Itemloaderfromshop>();
        sus1234.StartCoroutine(sus1234.GetJsonFromAddress());
        print(sus1234);

        foreach (skinloaderfromshop skinLoader in skinLoaders)
        {
            print("popopopopo");
            skinLoader.StartCoroutine(skinLoader.GetJsonFromAddress());
        }
    }

    public IEnumerator GetJsonFromAddress(string konec)
    {
        debug.text = "1";
        UnityWebRequest request = UnityWebRequest.Get(address + konec + "?id=" + login.id + "&pass=" + password);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);
            JsonData data = JsonUtility.FromJson<JsonData>(json);
            debug.text = "1.5";
            string ssuscoins = data.suscoins;
            suscoins = long.Parse(ssuscoins);
            suscoinsis.text = data.suscoins + "S";
            skinu = data.skins;

            skinloaderfromshop[] skinLoaders = FindObjectsOfType<skinloaderfromshop>();
            foreach (skinloaderfromshop skinLoader in skinLoaders)
            {
                skinLoader.StartCoroutine(skinLoader.GetJsonFromAddress());
            }
            FindObjectOfType<Itemloaderfromshop>().StartCoroutine(FindObjectOfType<Itemloaderfromshop>().GetJsonFromAddress());
            print(FindObjectOfType<Itemloaderfromshop>());
            debug.text = "2";

        }
    }

    [System.Serializable]
    private class JsonData
    {
        public string suscoins;
        public string skins;
    }
    public void pokupka(int id, long cena)
    {
        if (suscoins >= cena)
        {                
            suscoins -= cena;
            skinu = skinu + "," + id;
            print(skinu);
            StartCoroutine(GetJsonFromAddress2());
            suscoinsis.text = suscoins + "S";
            skinmanager228.spawnbuyedskinsmenu(id);
        }
    }

    private IEnumerator GetJsonFromAddress2()
    {
        debug.text = "3";
        UnityWebRequest request = UnityWebRequest.Get(address2 + "?id=" + login.id + "&suscoins=" + suscoins + "&skins=" + skinu + "&pass=" + password);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            string json2 = request.downloadHandler.text;
            Debug.Log(json2);
            JsonData2 data2 = JsonUtility.FromJson<JsonData2>(json2);
        }
        debug.text = "4";
    }

    [System.Serializable]
    private class JsonData2
    {
        public string success;
    }
}
