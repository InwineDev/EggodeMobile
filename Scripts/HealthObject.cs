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
        gameObject.transform.position = FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().dieCordReally;
        userSettings s = gameObject.GetComponent<userSettingNotCam>().us;
        if (FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().survival)
        {
            foreach (var item in s.items)
            {
                item.GetComponent<TipikalPredmet>().itemdat.amount = 0;
                item.GetComponent<TipikalPredmet>().itemdat.sus3.text = item.GetComponent<TipikalPredmet>().itemdat.amount.ToString() + " штук";
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