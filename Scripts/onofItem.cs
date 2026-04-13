using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onofItem : NetworkBehaviour
{
    [SerializeField] private GameObject targetObject;

    [SyncVar(hook = nameof(OnToggleChanged))]
    public bool isActive;

    [SerializeField] private MultiMusicSystem mms;

    [SyncVar]
    public TipikalPredmet s;

    private void Start()
    {
        s = GetComponent<TipikalPredmet>();
        mms = GetComponent<MultiMusicSystem>();
    }

    private void Update()
    {
        if (s.usersettingitems.player.escaped) return;
        if (isOwned && Input.GetMouseButtonDown(0))
        {
            ToggleCommand();
            if (mms) mms.PlayClip();
        }
    }

    [Command]
    private void ToggleCommand()
    {
        isActive = !isActive;
        ToggleObject(isActive);
    }

    [ClientRpc]
    private void ToggleObject(bool active)
    {
        targetObject.SetActive(active);
    }

    private void OnToggleChanged(bool oldValue, bool newValue)
    {
        targetObject.SetActive(newValue);
    }
}
