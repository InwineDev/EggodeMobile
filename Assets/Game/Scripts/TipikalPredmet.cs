using System.Collections;
using UnityEngine;
using Mirror;
using Unity.Burst.CompilerServices;
using System;
using TMPro;

//[RequireComponent(typeof(AudioSource))]
public class TipikalPredmet : NetworkBehaviour
{

    [Header("Item Settings")]
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField][Tooltip("Base damage per shot")] private int damage = 20;
    [Tooltip("Object to spawn")] public GameObject spawn;
    [SerializeField][Tooltip("Spawn prefab")] private bool spawned;
    [SerializeField][Tooltip("Spawn position offset")] private Vector3 pogreh;
    [SerializeField][Tooltip("Attack cooldown time")] private float kt = 1f;
    private bool ktbool;
    [SerializeField] private bool canDamage = true;

    //[Header("Networking")]
    [SyncVar]
    private bool noInited = true;
    private NetworkIdentity networkIdentity;

    [Header("Item Data")]
    [Tooltip("Dont touch")] public itemdannie itemdat;
    [Tooltip("Dont touch")] public int id;
    [Tooltip("Object name")] public string itemName;
    [Tooltip("Animation name")] public string animationName = "udar";
    [Tooltip("Dont touch")] public userSettings usersettingitems;
    [Tooltip("Preview")] public Sprite texture;
    [Tooltip("How use?")] [TextArea] public string helpText;

    [Header("References")]
    [Tooltip("Dont touch")] public GameObject player;
    [Tooltip("Dont touch")] public Animator animka;

    [Header("Sounds")]
    [SerializeField] private MultiMusicSystem mms;

    /*    [SyncVar(hook = nameof(SyncParent))]
        private uint parentNetId;

        void SyncParent(uint oldNetId, uint newNetId)
        {
            if (NetworkClient.spawned.TryGetValue(newNetId, out NetworkIdentity identity))
            {
                transform.SetParent(identity.transform);
                transform.localPosition = Vector3.zero;
            }
        }

        public void SetParentOnServer(NetworkIdentity newParent)
        {
            parentNetId = newParent.netId;
        }*/
    /*    private void Start()
        {
            if (!noInited)
            {
                init();
            }
        }*/
    public void init()
    {
        //gameObject.GetComponent<SyncParent>().SetParent(player.transform.parent);
        //usersettingitems = gameObject.transform.parent.transform.parent.transform.parent.GetComponent<userSettings>();
        mms = GetComponent<MultiMusicSystem>();
        GameObject cat = Instantiate(usersettingitems.item, usersettingitems.slider.transform);
        print(id);
        cat.GetComponent<itemdannie>().id = id;
        cat.GetComponent<itemdannie>().usersettingitems = usersettingitems;
        itemdat = cat.GetComponent<itemdannie>();
        networkIdentity = transform.parent.GetComponent<NetworkIdentity>();

        animka = gameObject.transform.parent.transform.parent.GetComponent<Animator>();
        //player = gameObject.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.gameObject;
        /*        networkIdentity = GetComponent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    NetworkIdentity itemIdentity = item.GetComponent<NetworkIdentity>();
                    itemIdentity.AssignClientAuthority(connectionToClient);
                }*/

        /*if (isOwned)
        {
            gameObject.SetActive(false);
        }*/
        //CmdChangeActive();
    }

    [Command]
    void CmdChangeActive()
    {
        gameObject.SetActive(false);
    }
    [Command]
    void CmdAddItem(ChestController cc)
    {
        if (isServer && cc != null)
        {
            cc.AddItem(id, usersettingitems);
        }
    }
    /*    [ClientRpc]
        void RpcAddItem(ChestController cc)
        {
            cc.AddItem(id, usersettingitems);
        }*/

    void Update()
    {
        if (!isOwned)
        {
            networkIdentity.AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
            return;
        }

/*        if (Input.GetKeyDown(KeyCode.X))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                ChestController cc = hit.transform.gameObject.GetComponent<ChestController>();
                if (cc != null)
                {
                    CmdAddItem(cc);
                    return;
                }
            }
        }*/
        if (Input.GetMouseButtonDown(1))
        {
            if (spawned)
            {
                if (!player.GetComponent<FirstPersonController>().escaped)
                {
                    if (serverProperties.instance.spawnn)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, raycastDistance))
                        {
                            Vector3 spawnPosition = hit.point + pogreh;
                            CmdSpawnCat(spawnPosition);
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (ktbool == false & canDamage == true)
            {
                if (!player.GetComponent<FirstPersonController>().escaped)
                {
                    if(animka) animka.Play(animationName);
                    StartCoroutine(kttime());
                    ktbool = true;
                    //StartCoroutine(zaderzka());
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, raycastDistance))
                    {
                        if (mms) mms.PlayClip();
                        if (hit.transform.gameObject.GetComponent<Health>() != null)
                        {
                            DAMA3GE(hit.transform.gameObject.GetComponent<Health>());
                        }
                        if (hit.transform.gameObject.GetComponent<name24>() != null)
                        {
                            DAMAGEITEM(hit.transform.gameObject.GetComponent<name24>());
                        }
                    }
                }
            }
        }
        }

    
/*    private IEnumerator zaderzka()
    {
        yield return new WaitForSeconds(0.001f);
        if (animka) animka.SetBool("udar", false);
    }*/

    [Command]
    void DAMAGEITEM(name24 sus)
    {

        bool uron2 = serverProperties.instance.destroy;
        if (uron2)
        {
            print("sus1");
            sus._hp -= damage;
        }
    }

        [Command]
        void DAMA3GE(Health sus)
        {
            bool uron = serverProperties.instance.hp;
            if (uron)
            {
                print("sus1");
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
        if (networkIdentity != null)
        {
            networkIdentity.AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
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

    [Command]
    void CmdSpawnCat(Vector3 hit)
    {
        GameObject cat = Instantiate(spawn, hit, player.transform.rotation);
        NetworkServer.Spawn(cat, connectionToClient);
        RpcSPAWNITEMS();
    }

/*    [Server]
    void ServerSpawn(Vector3 hit)
    {
        GameObject cat = Instantiate(spawn, hit, player.transform.rotation);
        NetworkServer.Spawn(cat, connectionToClient);
        RpcSPAWNITEMS();
    }*/


    [TargetRpc]
    void RpcSPAWNITEMS()
    {
        itemdat.RemoveItems(1);
        if (itemdat.amount <= 0)
        {
            usersettingitems.ChangeSkin(0);
        }
    }
}