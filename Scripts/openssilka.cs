using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openssilka : MonoBehaviour
{

    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;
    public void OpenYOUTUBE()
    {
        Application.OpenURL("https://www.youtube.com/@EPICSUSGAMES");
    }

    public void OpenTelegram()
    {
        Application.OpenURL("https://t.me/EPICSUSGAMES");
    }

    public void OpenSITE()
    {
        Application.OpenURL("https://www.epicsusgames.ru");
    }


    private IEnumerator crash()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1500f, 125000f));
        int sus245 = 0;
        Instantiate(button1, new Vector3(UnityEngine.Random.Range(-3.43f, 7.09f), UnityEngine.Random.Range(-1f, 5f), UnityEngine.Random.Range(-4.9f, 4.41f)), Quaternion.identity);
        while (true)
        {
            Instantiate(button2, new Vector3(UnityEngine.Random.Range(-3.43f, 7.09f), UnityEngine.Random.Range(-1f, 5f), UnityEngine.Random.Range(-4.9f, 4.41f)), Quaternion.identity);
            sus245++;
            if (sus245 <= 3)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.6f));
            }
        }

    }
}
