using Mirror;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstLaunch") != 1)
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        try
        {
            NetworkManager omg = FindObjectOfType<NetworkManager>();
            if (omg != null)
            {
                omg.onlineScene = "Tutorial";
                omg.StartHost();
            }
        }
        catch
        {
            PlayerPrefs.SetInt("FirstLaunch", 0);
        }
    }
}