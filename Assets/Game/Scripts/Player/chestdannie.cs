using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class chestdannie : NetworkBehaviour
{
    public int id;

    [SyncVar(hook= nameof(SetAmountText))]
    public int amount;

    public Sprite skinimage;
    public string nam1e;
    public Image sus;
    public TMP_Text sus2;
    public TMP_Text sus3;
    public userSettings usersettingitems;

    public bool binding;

    public KeyCode bind;

    public TMP_Text bindButton;

    [SerializeField] private GameObject param;

    public ChestController chest;

    public void Starting()
    {
        nam1e = usersettingitems.items[id].name;
        skinimage = usersettingitems.items[id].GetComponent<TipikalPredmet>().texture;
        sus2.text = nam1e;
        sus.sprite = skinimage;
        sus3.text = amount.ToString() + " ����";
        if(amount <= 0)
        {
            usersettingitems.ClearChest(gameObject);
        }
    }
    private void UpdateUI(int oldAmount, int newAmount)
    {
        // ��� ������� UI-�����
        sus3.text = newAmount.ToString() + " ����";

        if (isServer && newAmount <= 0)
        {
            usersettingitems.ClearChest(gameObject);
        }
    }

/*    public void getItem()
    {
        if (usersettingitems == null || chest == null)
        {
            Debug.LogError("Missing references!");
            return;
        }

        if (amount > 0 && isLocalPlayer)
        {
            CmdGetItem(id, usersettingitems);
        }
    }*/

/*    [Command]
    void CmdGetItem(int id1, userSettings usersettingsit)
    {
        amount = chest.GetItem(id1, usersettingsit);
    }*/
    void SetAmountText(int oldv, int newv)
    {
        sus3.text = newv.ToString() + " ����";
    }
}
