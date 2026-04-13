using Mirror;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ChatIdentityFix : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnValueChange))]
    public string syncedValue;

    public TMP_Text thistxt;

    private void OnValueChange(string oldValue, string newValue)
    {
        var sus = GameObject.Find("ChatVertical");
        Debug.Log($"Значение syncedValue изменено: {oldValue} -> {newValue}");
        thistxt.text = syncedValue;
        gameObject.transform.parent = GameObject.Find("ContentChat").transform;
        if (sus != null)
        {
            sus.GetComponent<Scrollbar>().value = 0;
        }
    }
}
