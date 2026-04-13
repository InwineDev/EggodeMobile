using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TTSChatController : NetworkBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text chat;

    [SyncVar(hook = nameof(OnChatTextChanged))]
    public string txtToChat;

    private void OnChatTextChanged(string oldText, string newText)
    {
        chat.text = newText;
        chat.gameObject.SetActive(!string.IsNullOrEmpty(newText));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
    }

    public void Send()
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            CmdSendMessage(input.text);
            input.text = "";
        }
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        txtToChat = message;
        RpcUpdateChat(message);
    }

    [ClientRpc]
    private void RpcUpdateChat(string message)
    {
        chat.text = message;
        chat.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }
    /*
        void syncTxt(string o, string n)
        {
            chat.text = n;
        }

        public void send()
        {
            chat.text = input.text;
            chat.gameObject.SetActive(true);
            CmdSend();
        }

        [Command]
        void CmdSend()
        {
            chat.text = input.text;
            chat.gameObject.SetActive(true);
            RpcSend();
        }

        [ClientRpc]
        void RpcSend()
        {
            chat.text = input.text;
            chat.gameObject.SetActive(true);
        }*/
}
