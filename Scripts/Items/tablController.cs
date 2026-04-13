using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class tablController : NetworkBehaviour, interactable
{
    [SerializeField] private GameObject me;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text txt;

    [SyncVar(hook = nameof(textSync))]
    public string textTexta;
    public void interact(FirstPersonController player)
    {
        player.escaped = true;
       /* player.GetComponent<userSettingNotCam>().us.canWrite = true;*/
        Cursor.lockState = CursorLockMode.None;
        me.SetActive(true);
        input.text = txt.text;
    }

    void textSync(string olld, string neww)
    {
        textTexta = neww;
        txt.text = neww;
        input.text = neww;
    }

/*    private void Update()
    {
        textTexta = input.text;
    }*/
    public void OnChangeText()
    {
        //textTexta = input.text;
        CmdChangeTxt(input.text);
    }

    [Command(requiresAuthority = false)]
    void CmdChangeTxt(string strah)
     {
         textTexta = strah;
         /*RpcChangeTxt(strah);*/
     }
/*
    [ClientRpc]
    void RpcChangeTxt(string strah)
    {
        textTexta = strah;
    }*/
}
