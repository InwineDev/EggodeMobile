using Mirror;
using TMPro;
using UnityEngine;
using System;
using System.Collections;

public class HealthObject : Health
{
    public override void serialHP(int oldHp, int mewHp)
    {
        if (health <= 0)
        {
            GameObject cat = Instantiate(spawn, transform.position, Quaternion.identity);
            NetworkServer.Spawn(cat);
            Die(spawn);
        }
    }

    public override IEnumerator blooding()
    {
        yield return new WaitForSeconds(1);
        blood.SetActive(false);
    }

    [TargetRpc]
    public override void Die(GameObject spawn1)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null)
        {
            gameObject.transform.position = props.dieCordReally;
        }

        userSettings s = gameObject.GetComponent<userSettingNotCam>().us;

        if (props != null && props.survival)
        {
            foreach (var item in s.items)
            {
                item.GetComponent<TipikalPredmet>().itemdat.amount = 0;
                item.GetComponent<TipikalPredmet>().itemdat.sus3.text =
                    item.GetComponent<TipikalPredmet>().itemdat.amount.ToString() + " шт";
            }
        }

        health = 100;
        CMD_TEXT();
        CMD_ZVUK();
        hp.text = $"{health} HP";
    }

    [Command]
    public override void CMD_TEXT()
    {
        Name = $"{health} HP";
    }

    [Command]
    public override void CMD_ZVUK()
    {
        source.Play(0);
    }

    public override void UpdateName(string oldName, string newName)
    {
        public_hp.text = newName;
    }
}