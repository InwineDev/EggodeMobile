using Mirror.Examples.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;

public class itemdannie : NetworkBehaviour
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
    [SerializeField] private TMP_Text idText;

    public Action<int> ChangeAmount;
    private void OnEnable()
    {
        Invoke("Starting", 0.1f);
    }
    public void RemoveItems(int howMany)
    {
        amount -= howMany;
        ChangeAmount?.Invoke(howMany);
        sus3.text = amount.ToString() + " штук";
    }
    public void Starting()
    {
        if (!serverProperties.instance.survival)
        {
            amount = 9999999;
        }
        //usersettingitems = FindObjectOfType<userSettings>();
        print(usersettingitems);
        nam1e = usersettingitems.items[id].GetComponent<SyncActive>().tpk.itemName;
        skinimage = usersettingitems.items[id].GetComponent<SyncActive>().tpk.texture;
        sus2.text = nam1e;
        sus.sprite = skinimage;
        sus3.text = amount.ToString() + " штук";
        if (settingsController.developer) idText.text = "ID: " + id;
        //if (amount <= 0) gameObject.SetActive(false);
        //TODO: Оптимизировать апдейт
        if (amount <= 0)
        {
            usersettingitems.ChangeSkin(0);
            gameObject.SetActive(false);
        }
    }

    public void dots()
    {
        if (param.activeSelf) param.SetActive(false);
        else param.SetActive(true);
    }
    public void bindButtonFunc()
    {
        binding = true;
        usersettingitems.canWrite = true;
        bindButton.text = "...";
    }
    private void Update()
    {
        if (!binding) return;
        if (Input.GetKey(Event.KeyboardEvent(Input.inputString).keyCode))
        {
            bind = Event.KeyboardEvent(Input.inputString).keyCode;
            bindButton.text = Input.inputString;
            binding = false;
            usersettingitems.canWrite = false;
        }
    }

    public void setitem()
    {
        if (amount > 0)
        {
            usersettingitems.ChangeSkin(id);
        }
    }

    void SetAmountText(int oldv, int newv)
    {
        sus3.text = newv.ToString() + " штук";
    }
}
