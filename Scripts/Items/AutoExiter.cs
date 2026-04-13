using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoExiter : NetworkBehaviour
{
    void OnEnable()
    {
        Invoke("bye", 10f);
    }

    void bye()
    {
        gameObject.SetActive(false);
        CmdBye();
    }

    [Command]
    void CmdBye()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcBye()
    {
        gameObject.SetActive(false);
    }
}
