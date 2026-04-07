using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SyncVar(hook = nameof(serialHP))]
    public int health = 100;

    public TMP_Text hp;

    [SyncVar(hook = nameof(UpdateName))]
    public string Name = "NULL";

    public TMP_Text public_hp;

    public GameObject blood;
    public GameObject spawn;
    public AudioSource source;
    public AudioClip dieSound;

    public Action<int, int> OnDamage;

    [Command]
    public void DAMA3GE(int damage)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.hp)
        {
            print("sus1");
            health -= damage + UnityEngine.Random.Range(0, 5);
            if (health <= 0)
            {
                health = 100;
                hp.text = $"{health} HP";
            }
        }
    }

    public virtual void serialHP(int oldHp, int mewHp)
    {
        hp.text = $"{health} HP";

        if (oldHp > mewHp)
        {
            blood.SetActive(true);
            StartCoroutine(blooding());
        }

        CMD_TEXT();

        if (health <= 0)
        {
            GameObject cat = Instantiate(spawn, transform.position, Quaternion.identity);
            NetworkServer.Spawn(cat);
            Die(spawn);
        }
    }

    public virtual IEnumerator blooding()
    {
        yield return new WaitForSeconds(1);
        blood.SetActive(false);
    }

    [TargetRpc]
    public virtual void Die(GameObject spawn1)
    {
        gameObject.transform.position = serverProperties.instance.dieCordReally;
        userSettings s = gameObject.GetComponent<userSettingNotCam>().us;

        if (serverProperties.instance.survival)
        {
            foreach (var item in s.items)
            {
                item.GetComponent<SyncActive>().tpk.itemdat.amount = 0;
                item.GetComponent<SyncActive>().tpk.itemdat.sus3.text =
                    item.GetComponent<TipikalPredmet>().itemdat.amount.ToString() + " шт";
            }
        }

        health = 100;
        CMD_TEXT();
        CMD_ZVUK();
        hp.text = $"{health} HP";
    }

    [Command]
    public virtual void CMD_TEXT()
    {
        Name = $"{health} HP";
    }

    [Command]
    public virtual void CMD_ZVUK()
    {
        RPC_ZVUK();
    }

    [ClientRpc]
    public void RPC_ZVUK()
    {
        source.clip = dieSound;
        source.Play(0);
    }

    public virtual void UpdateName(string oldName, string newName)
    {
        public_hp.text = newName;
    }
}