using Mirror;
using System.Collections;
using System.Collections.Generic;
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
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.destroy)
        {
            print("sus1");
            sus._hp -= damage + Random.Range(0, 5);
        }
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.hp)
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
        if (s == null) s = GetComponent<TipikalPredmet>();
        if (s == null || s.usersettingitems == null || s.usersettingitems.player == null) return;
        if (s.usersettingitems.player.escaped) return;
    }

    public void MobileLeftMouseDownAction()
    {
        if (!isOwned) return;
        CmdChangeEffectOn();
    }

    public void MobileLeftMouseHoldAction()
    {
        if (!isOwned) return;
        CmdShoot();
    }

    public void MobileLeftMouseUpAction()
    {
        if (!isOwned) return;
        CmdChangeEffectOff();
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
            if (ad != null && !ad.isPlaying)
                ad.Play();
            effect.SetActive(true);
        }
    }

    [ClientRpc]
    void ChangeEffectOff()
    {
        if (effect)
        {
            if (ad != null)
                ad.Stop();
            effect.SetActive(false);
        }
    }
}
