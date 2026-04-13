using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class skinmanager : MonoBehaviour
{
    public string address;
    public string password;
    public int[] num;
    public GameObject skin;
    public GameObject slider;
    public GameObject uncustpmize;
    public GameObject customize2;
    public static string[] sugoma;

    public IEnumerator GetJsonFromAddress()
    {
        UnityWebRequest request = UnityWebRequest.Get(address + "?id=" + login.id + "&pass=" + password);
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
            string numuncorrect = data.skins;
            string[] numuncorrectArray = numuncorrect.Split(',');
            string items2 = data.items;
            num = Array.ConvertAll(numuncorrectArray, int.Parse);
            spawnskinsmenu();
        }
    }

    public void ButtonGuest()
    {
        string numuncorrect = "0,1,2,4";
        string[] numuncorrectArray = numuncorrect.Split(',');
        string items2 = "0,1,2,4";
        num = Array.ConvertAll(numuncorrectArray, int.Parse);
        spawnskinsmenu();
    }
    [System.Serializable]
    private class JsonData
    {
        public string skins;
        public string items;
    }

    public void spawnskinsmenu()
    {
        for (int i = 0; i < num.Length; i++)
        {
            GameObject cat = Instantiate(skin, new Vector3(0, 0, 0), Quaternion.identity);
            cat.GetComponent<skindannie>().id = num[i];
            cat.transform.parent = slider.transform;
        }
    }

    public void spawnbuyedskinsmenu(int id)
    {
        GameObject cat = Instantiate(skin, new Vector3(0, 0, 0), Quaternion.identity);
        cat.transform.parent = slider.transform;
        cat.GetComponent<skindannie>().id = id;
    }

        public void openorclose()
    {
            if (customize2.activeSelf)
            {
            customize2.SetActive(false);
            uncustpmize.SetActive(true);
            }
            else
            {
            customize2.SetActive(true);
            uncustpmize.SetActive(false);
        }
    }
}
