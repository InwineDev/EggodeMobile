using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class chatcreator : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSyncedValueChanged))]
    public string syncedValue = "NULL";

    public TMP_Text textDisplay;
    public Button addButton;
    public TMP_InputField inputField;
    public GameObject item228;

    private string nick;

    private void OnSyncedValueChanged(string oldValue, string newValue)
    {
        Debug.Log($"syncedValue changed: {oldValue} -> {newValue}");

        if (textDisplay != null)
        {
            textDisplay.text = newValue;
        }
    }

    [Command]
    private void CmdAddText(string newText)
    {
        syncedValue += newText;
    }

    private void AddText()
    {
        if (!isOwned || inputField == null)
            return;

        string newText = inputField.text;
        CmdAddText(newText);
        inputField.text = string.Empty;
    }

    private void Start()
    {
        nick = login.username;

        if (isServer && item228 != null)
        {
            GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newObject);

            ChatIdentityFix chatFix = newObject.GetComponent<ChatIdentityFix>();
            if (chatFix != null)
            {
                chatFix.syncedValue = $"Игрок {nick} подключился к серверу.";
            }
        }
    }

    public void RpcSugoma()
    {
        if (isOwned && NetworkClient.isConnected)
        {
            CmdSugoma(inputField.text, nick);
        }
    }

    [Command]
    private void CmdSugoma(string text, string nickname)
    {
        if (item228 == null)
            return;

        GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
        NetworkServer.Spawn(newObject);

        ChatIdentityFix chatFix = newObject.GetComponent<ChatIdentityFix>();
        if (chatFix != null)
        {
            chatFix.syncedValue = $"<{nickname}> {text}";
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isServer && item228 != null)
        {
            GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newObject);

            ChatIdentityFix chatFix = newObject.GetComponent<ChatIdentityFix>();
            if (chatFix != null)
            {
                chatFix.syncedValue = $"Игрок {nick} покинул сервер.";
            }
        }
    }
}