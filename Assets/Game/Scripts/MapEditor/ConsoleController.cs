using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [Header("")]
    public List<ConsoleCommand> commands = new List<ConsoleCommand>();

    [Header("")]
    public TMP_InputField tmpro;
    public GameObject prefabMessage;
    public static ConsoleController cc;
    public ScrollRect scrollRect;
    [SerializeField] private GameObject MessageParent;
    public Color[] consoleColors;

    private Button ExportButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConsoleEnter(tmpro.text);
        }
    }
    private void Start()
    {
        ExportButton = FindObjectOfType<SaveMapNotStart>().gameObject.GetComponent<Button>();
        cc = this;
        AddMessage("Welcome to MapEditor!", 0);
    }
    public void AddMessage(string msg, int color)
    {
        GameObject message = Instantiate(prefabMessage);
        message.transform.parent = MessageParent.transform;
        message.transform.localScale = new Vector3(1,1,1);
        TMP_Text messageTxt = message.GetComponent<TMP_Text>();
        messageTxt.text = "[" + System.DateTime.Now + "] " + msg;
        messageTxt.color = consoleColors[color];
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    public void ConsoleEnter(string txt)
    {
        foreach (var item1 in commands)
        {
            if (txt.ToLower().Contains(item1.command))
            {
                Invoke(item1.method, 0f);
                return;
            }
        }
    }

    public void Exit()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }
    
    public void UnblockExport()
    {
        ExportButton.interactable = true;
    }

/*#if UNITY_EDITOR
    [CustomEditor(typeof(ConsoleController))]
    class EditorConsole : Editor
    {
        public override void OnInspectorGUI()
        {
            
        }
    }
#endif*/
}
