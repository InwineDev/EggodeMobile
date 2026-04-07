using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class login : MonoBehaviour
{
    public string address;
    public string address2;
    public string address3;
    public string address4;
    public string password;
    public TMP_InputField email;
    public TMP_InputField pass;
    private string[] pon;
    public static string id;
    public int hashator;
    public string skinu;
    public Animator anim;
    public static string username;
    public skinmanager sus;
    public shopsus sus2;
    public GameObject starversia;
    public GameObject zagruzka;
    public static string urlMap;
    public long suscoins;
    public TMP_Text suscoinsis;
    public skinmanager skinmanager228;
    public GameObject magaz;
    public TMP_Text nicknametxt;

    public menuManager menMan;
    public TMP_Text versiontxt;
    public TMP_Text errortxt;
    public TMP_Text errordebuglogintxt;

    public TMP_Text banprichina;
    public GameObject angryban;
    public string url;
    public Image img;
    public TMP_Text nameusera;
    public GameObject account;
    public string[] naruki;
    public string narukistring;

    public static int skindannaia = 0;

    private int goingnotzagruzka = 0;

    private void Start()
    {
        print(address);
        versiontxt.text = menMan.version;
        Cursor.lockState = CursorLockMode.None;
        zagruzka.SetActive(true);

        if (PlayerPrefs.HasKey("abobusisus"))
        {
            string dId = PlayerPrefs.GetString("abobusisus");
            id = DecryptString(dId);
            print("loaded!");
            anim.SetBool("sus1", true);

            if (id == "Guest")
            {
                ButtonGuestLoginButton();
            }
            else
            {
                StartCoroutine(GetJsonFromAddress2());
                StartCoroutine(sus.GetJsonFromAddress());
            }
        }
        else
        {
            zagruzka.SetActive(false);
            account.SetActive(false);
        }

        FindObjectOfType<userSettingNotCam>().OnSkinChangedNoMP(skindannaia);
    }

    public void ButtonLogin()
    {
        StartCoroutine(GetJsonFromAddress());
    }

    public void register()
    {
        Application.OpenURL("https://epicsusgames.ru/register");
    }

    public void ButtonGuestLoginButton()
    {
        StartCoroutine(ButtonGuestLogin());
    }

    public IEnumerator ButtonGuestLogin()
    {
        id = "Guest";
        errordebuglogintxt.text = "Гость: " + id;

        string encryptedId = EncryptString(id);
        Saver(encryptedId);

        anim.SetBool("sus", true);

        username = "Guest" + UnityEngine.Random.Range(0, 100000);
        nicknametxt.text = username;

        string vers = menMan.version;
        string ssuscoins = "0";
        suscoins = long.Parse(ssuscoins);
        suscoinsis.text = "0S";
        skinu = "0,1,2,4";
        url = "https://t4.ftcdn.net/jpg/07/40/34/41/360_F_740344153_IQT8C4zfJj81LWvYcBfCpQXaO1lNeIXW.jpg";

        string ban = "0";
        string banprichina2 = "GUEST ERROR!";

        if (vers != menMan.version)
        {
            starversia.SetActive(true);
        }

        print(username);
        yield return new WaitForSeconds(4);

        zagruzka.SetActive(false);
        magaz.SetActive(false);
        StartCoroutine(Pon());
        nameusera.text = username;

        if (ban == "1")
        {
            angryban.SetActive(true);
            banprichina.text = "Причина: " + banprichina2;
        }

        if (goingnotzagruzka == 0)
        {
            zagruzka.SetActive(true);
        }

        sus.ButtonGuest();
    }

    private IEnumerator GetJsonFromAddress()
    {
        UnityWebRequest request = UnityWebRequest.Get(
            address + "?email=" + email.text + "&password=" + pass.text + "&pass=" + password
        );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("Error: " + request.error);
            errortxt.text = "Error: " + request.error;
            errordebuglogintxt.text = "Error: " + request.error;
        }
        else
        {
            string json = request.downloadHandler.text;
            print(json);

            JsonData data = JsonUtility.FromJson<JsonData>(json);
            id = data.id;

            errordebuglogintxt.text = "Error: " + id;

            if (id != null)
            {
                errordebuglogintxt.text = "Успешно: " + id;

                string encryptedId = EncryptString(id);
                Saver(encryptedId);

                anim.SetBool("sus", true);

                StartCoroutine(GetJsonFromAddress2());
                StartCoroutine(sus.GetJsonFromAddress());
            }
        }
    }

    private string EncryptString(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] keyBytes = Encoding.UTF8.GetBytes(password);

        for (int i = 0; i < inputBytes.Length; i++)
        {
            inputBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return Convert.ToBase64String(inputBytes);
    }

    private string DecryptString(string input)
    {
        byte[] inputBytes = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.UTF8.GetBytes(password);

        for (int i = 0; i < inputBytes.Length; i++)
        {
            inputBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return Encoding.UTF8.GetString(inputBytes);
    }

    public void Saver(string encryptedId)
    {
        PlayerPrefs.SetString("abobusisus", encryptedId);
        PlayerPrefs.Save();
        print("saved!");
    }

    [System.Serializable]
    private class JsonData
    {
        public string id;
    }

    private IEnumerator GetJsonFromAddress2()
    {
        UnityWebRequest request = UnityWebRequest.Get(address2 + "?id=" + id + "&pass=" + password);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("Error: " + request.error);
            errortxt.text = "Error: " + request.error;
        }
        else
        {
            string json2 = request.downloadHandler.text;
            print(json2);

            JsonData2 data2 = JsonUtility.FromJson<JsonData2>(json2);
            username = data2.username;
            nicknametxt.text = username;

            string vers = data2.version;
            string ssuscoins = data2.suscoins;
            suscoins = long.Parse(ssuscoins);
            suscoinsis.text = data2.suscoins + "S";
            skinu = data2.skins;
            url = data2.img;
            string ban = data2.ban;
            string banprichina2 = data2.banprichina;

            if (vers != menMan.version)
            {
                starversia.SetActive(true);
            }

            print(username);
            yield return new WaitForSeconds(4);

            zagruzka.SetActive(false);
            magaz.SetActive(false);
            StartCoroutine(Pon());
            nameusera.text = username;

            if (ban == "1")
            {
                angryban.SetActive(true);
                banprichina.text = "Причина: " + banprichina2;
            }

            if (goingnotzagruzka == 0)
            {
                zagruzka.SetActive(true);
            }
        }
    }

    IEnumerator Pon()
    {
        WWW www = new WWW("https://www.epicsusgames.ru/" + url);
        yield return www;

        if (www.error == null)
        {
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGB24, false);
            www.LoadImageIntoTexture(texture);

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0, 0)
            );

            img.sprite = sprite;
            account.SetActive(false);
            goingnotzagruzka++;
            zagruzka.SetActive(false);
        }
        else
        {
            Debug.LogError("Error loading image: " + www.error);
            account.SetActive(false);
            goingnotzagruzka++;
            zagruzka.SetActive(false);
        }
    }

    [System.Serializable]
    private class JsonData2
    {
        public string ban;
        public string banprichina;
        public string username;
        public string success;
        public string version;
        public string suscoins;
        public string skins;
        public string img;
    }

    public void pokupka(int id, long cena)
    {
        if (suscoins >= cena)
        {
            suscoins -= cena;
            skinu += "," + id;
            print(skinu);

            StartCoroutine(GetJsonFromAddress23(skinu, suscoins));
            suscoinsis.text = suscoins + "S";
            skinmanager228.spawnbuyedskinsmenu(id);
        }
    }

    private IEnumerator GetJsonFromAddress23(string skin1, long suscoin)
    {
        UnityWebRequest request = UnityWebRequest.Get(
            address3 + "?id=" + login.id + "&suscoins=" + suscoin + "&skins=" + skin1 + "&pass=" + password
        );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("Error: " + request.error);
            errortxt.text = "Error: " + request.error;
        }
        else
        {
            string json23 = request.downloadHandler.text;
            print(json23);
            JsonData23 data23 = JsonUtility.FromJson<JsonData23>(json23);
        }
    }

    public void pokupka2(int id, long cena)
    {
        if (suscoins >= cena)
        {
            suscoins -= cena;
            narukistring = narukistring + "," + id;

            List<string> list = new List<string>();
            list.Add(id.ToString());
            skinmanager.sugoma = skinmanager.sugoma.Concat(list).ToArray();

            print(narukistring);
            StartCoroutine(GetJsonFromAddress24(narukistring, suscoins));
            suscoinsis.text = suscoins + "S";
        }
    }

    private IEnumerator GetJsonFromAddress24(string skin1, long suscoin)
    {
        UnityWebRequest request = UnityWebRequest.Get(
            address4 + "?id=" + login.id + "&suscoins=" + suscoin + "&items=" + skin1 + "&pass=" + password
        );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("Error: " + request.error);
            errortxt.text = "Error: " + request.error;
        }
        else
        {
            string json24 = request.downloadHandler.text;
            print(json24);
            JsonData23 data24 = JsonUtility.FromJson<JsonData23>(json24);
        }
    }

    [System.Serializable]
    private class JsonData23
    {
        public string success;
    }

    [System.Serializable]
    private class JsonData24
    {
        public string success;
    }
}