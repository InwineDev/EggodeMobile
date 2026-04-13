using Mirror;
using TMPro;
using UnityEngine;
using System;
using System.Collections;

public class HealthPlayer : Health
{
    [SerializeField] private MultiMusicSystem mms;

    private void Start()
    {
        mms = GetComponent<MultiMusicSystem>();
    }
    public override void serialHP(int oldHp, int newHp)
    {
        OnDamage?.Invoke(oldHp, newHp);

        hp.text = $"{health} HP";
        if(oldHp > newHp)
        {
            blood.SetActive(true);
            CmdUron();
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

    [Command]
    public void CmdUron()
    {
        source.clip = mms.GetRandomAudio();
        source.Play(0);
    }

    public override IEnumerator blooding()
    {
        yield return new WaitForSeconds(1);
        blood.SetActive(false);
    }

    [TargetRpc]
    public override void Die(GameObject spawn1)
    {
        userSettings s = gameObject.GetComponent<userSettingNotCam>().us;
        s.StandFromVzaim();
        if (serverProperties.instance.survival)
        {
            foreach (var item in s.items)
            {
                SyncActive strah = item.GetComponent<SyncActive>();
                strah.tpk.itemdat.amount = 0;
                strah.tpk.itemdat.sus3.text = strah.tpk.itemdat.amount.ToString() + " штук";
            }
            s.ChangeSkin(0);
        }

        health = 100;
        CMD_TEXT();
        CMD_ZVUK();
        hp.text = $"{health} HP";
        gameObject.transform.position = serverProperties.instance.dieCordReally;
    }


    [Command]
    public override void CMD_TEXT()
    {
        Name = $"{health} HP";
    }

    [Command]
    public override void CMD_ZVUK()
    {
        source.clip = dieSound;
        source.Play(0);
    }
    public override void UpdateName(string oldName, string newName)
    {
        public_hp.text = newName;
    }

}