using System;
using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

public class TipikalPredmet : NetworkBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private int damage = 20;
    public GameObject spawn;
    [SerializeField] private bool spawned;
    [SerializeField] private Vector3 pogreh;
    [SerializeField] private float kt = 1f;
    private bool ktbool;
    [SerializeField] private bool canDamage = true;

    [SyncVar] private bool noInited = true;
    private NetworkIdentity networkIdentity;

    [Header("Item Data")]
    public itemdannie itemdat;
    public int id;
    public string itemName;
    public string animationName = "udar";
    public userSettings usersettingitems;
    public Sprite texture;
    [TextArea] public string helpText;

    [Header("References")]
    public GameObject player;
    public Animator animka;

    [Header("Sounds")]
    [SerializeField] private MultiMusicSystem mms;

    public void init()
    {
        mms = GetComponent<MultiMusicSystem>();

        GameObject cat = Instantiate(usersettingitems.item, usersettingitems.slider.transform);
        cat.GetComponent<itemdannie>().id = id;
        cat.GetComponent<itemdannie>().usersettingitems = usersettingitems;
        itemdat = cat.GetComponent<itemdannie>();

        networkIdentity = transform.parent.GetComponent<NetworkIdentity>();
        animka = gameObject.transform.parent.transform.parent.GetComponent<Animator>();
    }

    void Update()
    {
        if (!isOwned)
        {
            if (networkIdentity != null && player != null)
            {
                var playerNetId = player.GetComponent<NetworkIdentity>();
                if (playerNetId != null && playerNetId.connectionToClient != null)
                {
                    networkIdentity.AssignClientAuthority(playerNetId.connectionToClient);
                }
            }
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            UseSecondary();
        }

        if (Input.GetMouseButtonDown(0))
        {
            UsePrimary();
        }
    }

    public void UseSecondary()
    {
        if (!CanUseSecondary()) return;

        Camera targetCam = GetPlayerCamera();
        if (targetCam == null) return;

        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = targetCam.ScreenPointToRay(center);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Vector3 spawnPosition = hit.point + pogreh;
            CmdSpawnCat(spawnPosition);
        }
    }

    public void UsePrimary()
    {
        if (!isOwned) return;
        if (ktbool) return;
        if (!canDamage) return;
        if (player == null) return;

        FirstPersonController fpc = player.GetComponent<FirstPersonController>();
        if (fpc != null && fpc.escaped) return;

        if (animka) animka.Play(animationName);

        StartCoroutine(kttime());
        ktbool = true;

        Camera targetCam = GetPlayerCamera();
        if (targetCam == null) return;

        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = targetCam.ScreenPointToRay(center);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            if (mms) mms.PlayClip();

            Health hp = hit.transform.GetComponent<Health>();
            if (hp != null)
            {
                DAMA3GE(hp);
            }

            name24 itemHp = hit.transform.GetComponent<name24>();
            if (itemHp != null)
            {
                DAMAGEITEM(itemHp);
            }
        }
    }

    private bool CanUseSecondary()
    {
        if (!isOwned) return false;
        if (!spawned) return false;
        if (player == null) return false;
        if (serverProperties.instance == null) return false;
        if (!serverProperties.instance.spawnn) return false;

        FirstPersonController fpc = player.GetComponent<FirstPersonController>();
        if (fpc != null && fpc.escaped) return false;

        if (itemdat == null) return false;
        if (itemdat.amount <= 0) return false;

        return true;
    }

    private Camera GetPlayerCamera()
    {
        if (usersettingitems != null && usersettingitems.cam != null)
            return usersettingitems.cam;

        return Camera.main;
    }

    [Command]
    void CmdSpawnCat(Vector3 hit)
    {
        if (spawn == null) return;

        GameObject cat = Instantiate(spawn, hit, player.transform.rotation);
        NetworkServer.Spawn(cat, connectionToClient);
        RpcSPAWNITEMS();
    }

    [TargetRpc]
    void RpcSPAWNITEMS()
    {
        itemdat.RemoveItems(1);
        if (itemdat.amount <= 0)
        {
            usersettingitems.ChangeSkin(0);
        }
    }

    [Command]
    void DAMAGEITEM(name24 sus)
    {
        bool uron2 = serverProperties.instance.destroy;
        if (uron2)
        {
            sus._hp -= damage;
        }
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        bool uron = serverProperties.instance.hp;
        if (uron)
        {
            sus.health -= damage;
            if (sus.health <= 0)
            {
                sus.health = 100;
                sus.hp.text = $"{sus.health} HP";
            }
        }
    }

    private void OnEnable()
    {
        if (networkIdentity != null && player != null)
        {
            var playerNetId = player.GetComponent<NetworkIdentity>();
            if (playerNetId != null && playerNetId.connectionToClient != null)
            {
                networkIdentity.AssignClientAuthority(playerNetId.connectionToClient);
            }
        }

        if (ktbool)
        {
            StartCoroutine(kttime());
        }
        else
        {
            usersettingitems.OnKtStart?.Invoke(0);
        }
    }

    private IEnumerator kttime()
    {
        usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
    }
}