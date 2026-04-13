using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class FlamethrowerController : NetworkBehaviour
{
    public AudioSource ad;
    [SerializeField] private float kt = 1f;
    public bool ktbool;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private int damage = 15;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject effectToSpawn;

    [SerializeField] private Animator animator;

    [SyncVar]
    public TipikalPredmet s;

    [Command]
    void CmdShoot()
    {
        RpcSpawn();
    }

    [ClientRpc]
    void RpcSpawn()
    {

    }

    [Command]
    void DAMAGEITEM(name24 sus)
    {

        bool uron2 = FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().destroy;
        if (uron2)
        {
            print("sus1");
            sus._hp -= damage + Random.Range(0, 5);
        }
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        bool uron = FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().hp;
        if (uron)
        {
            print("sus1");
            sus.health -= damage + Random.Range(0, 5);
            if (sus.health <= 0)
            {
                sus.health = 100;
                sus.hp.text = $"{sus.health} HP";
            }
        }
    }
    void Update()
    {
        if (s.usersettingitems.player.escaped) return;
        if(isOwned && Input.GetMouseButtonDown(0))
        {
            CmdChangeEffectOn();
        }
        if (isOwned && Input.GetMouseButton(0))
        {
                CmdShoot();
        }
        if (isOwned && Input.GetMouseButtonUp(0))
        {
            CmdChangeEffectOff();
        }
    }

    private void OnEnable()
    {

    }

    [Command]
    void CmdChangeEffectOn()
    {
        ChangeEffectOn();
    }
    [Command]
    void CmdChangeEffectOff()
    {
        ChangeEffectOff();
    }
    [ClientRpc]
    void ChangeEffectOn()
    {
        if (effect)
        {
            ad.Play();
            effect.SetActive(true);
        }
    }
    [ClientRpc]
    void ChangeEffectOff()
    {
        if (effect)
        {
            ad.Stop();
            effect.SetActive(false);
        }
    }
}