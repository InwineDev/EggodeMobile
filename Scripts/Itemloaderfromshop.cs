using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Itemloaderfromshop : MonoBehaviour
{
    public string address;
    public int whatskin = 0;
    public GameObject zagruzka;
    public login sus;
    public long cena = 10;
    public TMP_Text cenik;
    public Button but;
    public static string[] sugoma = { "0" };
    public bool zagruzka228 = true;
    public GameObject SHOP;

    private void Start()
    {
        if (login.id != null)
        {
            zagruzka.SetActive(true);
        }
        else
        {
            zagruzka.SetActive(false);
        }
    }

    private void Update()
    {
        if (login.id != null)
        {
            if (zagruzka228)
            {
                zagruzka.SetActive(true);
            }
        }
    }

    public IEnumerator GetJsonFromAddress()
    {
        if (sus.suscoins > -1)
        {
            print(sus.suscoins);
            UnityWebRequest request = UnityWebRequest.Get(address);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                print("Error: " + request.error);
                sus.errortxt.text = "Error: " + request.error;
            }
            else
            {
                string json = request.downloadHandler.text;
                print(json);
                JsonData data = JsonUtility.FromJson<JsonData>(json);
                string skins = data.skin;
                string[] skin = skins.Split(',');
                string ceni = data.ceni;
                string[] cen = ceni.Split(',');
                cena = int.Parse(cen[whatskin]);
                string[] skin22 = sus.narukistring.Split(',');
                print(skin22);

                gameObject.GetComponent<itemdannie>().id = int.Parse(skin[whatskin]);
                gameObject.GetComponent<itemdannie>().Starting();

               print(sus.naruki);
                print("SUS");

                print("SUS3");
                foreach (string omg227 in skin22)
                {
                    if(omg227 == gameObject.GetComponent<itemdannie>().id.ToString())
                    {
                        but.interactable = false;
                        print("slava bogu");
                    }
                }

                zagruzka.SetActive(true);
                cenik.text = cena + "S";

                yield return 3f;

                zagruzka228 = false;
                zagruzka.SetActive(false);

            }
        }
    }

    [System.Serializable]
    private class JsonData
    {
        public string skin;
        public string ceni;
    }

    public void buy()
    {
        if (sus.suscoins >= cena)
        {
            but.interactable = false;
            sus.pokupka2(gameObject.GetComponent<itemdannie>().id, cena);
        }
    }
}
