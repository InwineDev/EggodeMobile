using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class nick : NetworkBehaviour
{
    [SyncVar(hook = nameof(UpdateName))]
    public string Name = "NULL";
    public TMP_Text me;
    public bool sus = true;
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            if (sus)
            {

                CmdUpdateName(settingsController.nickname);
            }

        }
    }

    [Command]
    private void CmdUpdateName(string newName)
    {
        Name = newName;
    }

    private void UpdateName(string oldName, string newName)
    {
        me.text = newName;
    }
}
