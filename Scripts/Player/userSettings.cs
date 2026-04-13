using Mirror;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class userSettings : NetworkBehaviour
{
    public Camera cam;
    public AudioListener cam1;
    public GameObject skin;
    public GameObject item;
    public GameObject nickname;
    public GameObject canvas;
    public KeyCode destroyKey = KeyCode.Z;
    public KeyCode useKey = KeyCode.E;

    public GameObject hptxt;
    public GameObject nametxt;

    public GameObject[] item228;
    public static int int1 = 0;
    public float raycastDistance = 5f;
    public GameObject slider;

    public GameObject[] npcOkno;

    [SyncVar(hook = nameof(OnWormChanged))]
    public int worm;

    public GameObject wormObject;

    public FirstPersonController player;

    public SyncList<TipikalPredmet> idannie = new SyncList<TipikalPredmet>();

    public bool canWrite = false;

    [Header("Chests")]
    public GameObject contentChest;
    public GameObject chestItemPrefab;
    public GameObject chestWindow;
    public List<GameObject> chestItemList;

    public Action<int> OnChangeItem;
    public Action<float> OnKtStart;

    [SerializeField] private KtController ktController;

    [Header("Drop system")]
    [SerializeField] private GameObject droppedPrefab;

    [SerializeField] private GameObject ruki;

    private void Awake()
    {
        if (ruki == null)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == "Ruki")
                {
                    ruki = child.gameObject;
                    break;
                }
            }
        }
    }

    private void OnEnable()
    {
        OnKtStart += ktController.PlayCenterAnimation;
    }

    private void OnDisable()
    {
        OnKtStart -= ktController.PlayCenterAnimation;
    }

    public void AddItem(int id)
    {
        AddItem(id, 1);
    }
    public void AddItem(int id, int amount)
    {
        idannie[id].GetComponent<TipikalPredmet>().itemdat.amount += amount;
    }
    public void OnWormChanged(int oldv, int newv)
    {
        if (newv >= 98)
        {
            wormObject.SetActive(true);
        }
    }

    void Start()
    {
        spawnitemmenu();

        StartCoroutine(OMG());

        if (!isLocalPlayer)
        {
            cam.enabled = false;
            cam1.enabled = false;
            canvas.SetActive(false);
            skin.SetActive(true);
        }
        else
        {
            cam.enabled = true;
            cam1.enabled = true;
            SetLayerToChildren(skin, 7);
            canvas.SetActive(true);
            worm = UnityEngine.Random.Range(1, 50);
        }
    }
    void SetLayerToChildren(GameObject parent, int newLayer)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.layer = newLayer;
            SetLayerToChildren(child.gameObject, newLayer);
        }
    }
    public IEnumerator OMG()
    {
        yield return new WaitForSeconds(3f);
        if (isLocalPlayer)
        {
            hptxt.SetActive(false);
            nametxt.SetActive(false);
        }
        else
        {
            hptxt.SetActive(true);
            nametxt.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcAddItemToList(GameObject item)
    {
        if (!isServer && isLocalPlayer)
        {
            items.Add(item);
        }
    }

    [Server]
    public void spawnitemmenu()
    {
        for (int i = 0; i < itemsPrefabs.Count; i++)
        {
            GameObject item = Instantiate(itemsPrefabs[i]);
            NetworkServer.Spawn(item, connectionToClient);
            item.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

            TipikalPredmet tp = item.GetComponent<SyncActive>().tpk;
            tp.id = i;
            tp.usersettingitems = this;
            tp.player = player.gameObject;
            idannie.Add(tp);
            items.Add(item);
            StartCoroutine(InitItem(item, i));
        }
        for (int i = 0; i < ModLoader.instance.objectsItems.Count; i++)
        {
            try
            {
                GameObject item = Instantiate(ModLoader.instance.objectsItems[i].itemInArm);
                NetworkServer.Spawn(item, connectionToClient);
                item.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
                int id = itemsPrefabs.Count + i;
                TipikalPredmet tp = item.GetComponent<SyncActive>().tpk;
                tp.id = id;

                tp.usersettingitems = this;
                tp.player = player.gameObject;
                tp.spawn = ModLoader.instance.objectsItems[i].spawnedItem;
                items.Add(item);
                idannie.Add(tp);

                StartCoroutine(InitItem(item, id));
            }
            catch (Exception ex)
            {
                print(ex);
            }
        }
    }

    [Server]
    IEnumerator InitItem(GameObject item, int index)
    {
        yield return new WaitForSeconds(0.1f);

        item.transform.SetParent(ruki.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = new Quaternion(0, 0, 0, 0);

        TipikalPredmet tp = item.GetComponent<SyncActive>().tpk;
        tp.player = player.gameObject;
        tp.usersettingitems = this;
        tp.id = index;
        tp.init();
        item.GetComponent<SyncActive>().SetActiv(index == 0);
        currentItemIndex = 0;
        RpcInitItem(item, index, index == 0);
    }

    [ClientRpc]
    void RpcInitItem(GameObject item, int i, bool active)
    {
        if (isServer) return;

        // --- ������ �� ������ ---
        if (item == null)
        {
            Debug.LogError($"[����] ������� �� ����� ������������ �� �������. ������: {i}");
            return;
        }

        if (ruki == null)
        {
            Debug.LogError("[UNITY] ���������� 'ruki' �� ��� ������! ���������, ����� �� ������ ���������� 'Ruki' � ������� ����� � ��������.");
            return;
        }

        SyncActive syncAct = item.GetComponent<SyncActive>();
        if (syncAct == null) return;
        // ------------------------

        item.transform.SetParent(ruki.transform);
        item.transform.localPosition = Vector3.zero;

        TipikalPredmet tp = syncAct.tpk;
        tp.usersettingitems = this;
        tp.player = player.gameObject;
        tp.id = i;
        tp.init();
        syncAct.SetActiv(active);
    }

    public void ClearChest(GameObject item1)
    {
        chestItemList.Remove(item1);
        Destroy(item1);
    }
    public void ClearAllChest()
    {
        List<GameObject> itemsToDestroy = new List<GameObject>(chestItemList);
        chestItemList.Clear();
        foreach (var item in itemsToDestroy)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
    }

    public GameObject f1;
    private int sittingNumber;
    public List<TipikalPredmet> localItems;
    public void StandFromVzaim()
    {
        if (!sitting) return;
        sitting.sittingPlayers--;
        player.transform.parent = null;
        ChangeRigidAndTrigger();
        sitting.tag = "object";
        sitting.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        sitting = null;
    }
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (sitting != null)
        {
            player.transform.localPosition = sitting.sittingPosition[sittingNumber];
        }

        if (canWrite) return;
        if (!player.escaped)
        {
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if (scrollWheel != 0)
            {
                List<TipikalPredmet> availableItems = new List<TipikalPredmet>();
                foreach (var item in idannie)
                {
                    if (item != null && item.itemdat != null && item.transform.parent != null)
                    {
                        if (item.itemdat.amount > 0)
                            availableItems.Add(item);
                    }
                }

                if (availableItems.Count == 0)
                    return;

                if (scrollWheel < 0)
                {
                    int nextIndex = -1;
                    for (int i = 0; i < availableItems.Count; i++)
                    {
                        int itemIndex = items.IndexOf(availableItems[i].transform.parent.gameObject);
                        if (itemIndex > currentItemIndex)
                        {
                            nextIndex = itemIndex;
                            break;
                        }
                    }
                    int fallbackIndex = items.IndexOf(availableItems[0].transform.parent.gameObject);
                    ChangeSkin(nextIndex != -1 ? nextIndex : fallbackIndex);
                }
                else if (scrollWheel > 0)
                {
                    int prevIndex = -1;
                    for (int i = availableItems.Count - 1; i >= 0; i--)
                    {
                        int itemIndex = items.IndexOf(availableItems[i].transform.parent.gameObject);
                        if (itemIndex < currentItemIndex)
                        {
                            prevIndex = itemIndex;
                            break;
                        }
                    }
                    int fallbackIndex = items.IndexOf(availableItems[availableItems.Count - 1].transform.parent.gameObject);
                    ChangeSkin(prevIndex != -1 ? prevIndex : fallbackIndex);
                }
            }
        }
        try
        {
            foreach (var item1 in idannie)
            {
                if (Input.GetKeyDown(item1.itemdat.bind))
                {
                    item1.itemdat.setitem();
                }
            }
        }
        catch
        {
            print("OKAY");
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (f1.activeSelf)
            {
                f1.SetActive(false);
            }
            else
            {
                f1.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject dropped = Instantiate(droppedPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(dropped);
            dropped.GetComponent<DroppedObjectController>().id = currentItemIndex;
        }

        if (Input.GetKeyDown(destroyKey))
        {
            if (serverProperties.instance.destroy)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.CompareTag("object"))
                    {
                        CmdDestroyObject(hitObject);
                    }
                }
            }
        }

        if (Input.GetKeyDown(useKey))
        {
            if (sitting != null)
            {
                StandFromVzaim();
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    SittingController vzObject = hitObject.GetComponent<SittingController>();
                    if (vzObject != null && vzObject.sittingPlayers <= vzObject.sittingPosition.Length)
                    {
                        sitting = vzObject;
                        sitting.tag = "Untagged";
                        player.transform.SetParent(hitObject.transform);
                        sittingNumber = vzObject.sittingPlayers;
                        player.transform.localPosition = sitting.sittingPosition[sittingNumber];
                        if (vzObject.sittingPlayers == 0) AssignSitting(sitting.GetComponent<NetworkIdentity>());
                        vzObject.sittingPlayers++;
                        ChangeRigidAndTrigger();
                    }
                    scriptor sc = hitObject.GetComponent<scriptor>();

                    if (sc)
                    {
                        CallInteractObject(sc);
                    }
                }
            }
            RaycastHit hit2;
            if (Physics.Raycast(transform.position, transform.forward, out hit2, raycastDistance))
            {
                GameObject hitObject = hit2.collider.gameObject;
                interactable vzObject = hitObject.GetComponent<interactable>();
                if (vzObject != null)
                {
                    vzObject.interact(player);
                }
            }
        }
    }

    [Command]
    void ChangeRigidAndTrigger()
    {
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        Collider collider = player.GetComponent<Collider>();
        rigidbody.isKinematic = !rigidbody.isKinematic;
        collider.isTrigger = !collider.isTrigger;
    }

    [ClientRpc]
    void ChangeRpcRigidAndTrigger()
    {
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        Collider collider = player.GetComponent<Collider>();
        rigidbody.isKinematic = !rigidbody.isKinematic;
        collider.isTrigger = !collider.isTrigger;
    }

    [Command]
    void AssignSitting(NetworkIdentity neti)
    {
        neti.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CallInteractObject(scriptor s)
    {
        CallInteractOnClients(s);
    }

    [ClientRpc]
    void CallInteractOnClients(scriptor s)
    {
        if (!isOwned) return;
        if (s.type == 1)
        {
            s.typeController(Player228);
        }
    }
    public void OnEnterNpc()
    {
        player.escaped = false;
        npcOkno[0].SetActive(false);
    }

    void IDEM1(SittingController o, SittingController d)
    {
        print("RPC");
        RPCSUS(d);
    }

    [TargetRpc]
    void RPCSUS(SittingController d)
    {
        if (d != null)
        {
            Player228.transform.parent = d.transform;
        }
    }

    [SyncVar]
    public SittingController sitting;

    public GameObject Player228;

    [Command]
    void CmdDestroyObject(GameObject objToDestroy)
    {
        NetworkServer.Destroy(objToDestroy);
    }

    [Command]
    void CmdSpawnKoshka(int prefabToSpawn, Vector3 hit)
    {
        print(prefabToSpawn);
        if (prefabToSpawn != 0)
        {
            GameObject koshka = Instantiate(item228[prefabToSpawn], hit, Quaternion.identity);
            NetworkServer.Spawn(koshka);
        }
    }

    [SyncVar(hook = nameof(OnInvChanged))]
    private int currentItemIndex = 10;

    public List<GameObject> itemsPrefabs = new List<GameObject>();
    public SyncList<GameObject> items = new SyncList<GameObject>();
    public Sprite[] s;
    public AudioSource changeItem;
    public MultiMusicSystem punchAudio;
    public void ChangeSkin(int newSkinIndex)
    {
        if (items[newSkinIndex].GetComponent<SyncActive>().tpk.itemdat.amount <= 0 & newSkinIndex != 0) return;
        CmdChangeSkin(newSkinIndex);
        OnChangeItem?.Invoke(newSkinIndex);
    }

    public void PunchPlay()
    {
        punchAudio.PlayClip();
    }

    [Command]
    private void CmdChangeSkin(int newSkinIndex)
    {
        currentItemIndex = newSkinIndex;
        changeItem.Play(0);
    }

    void OnInvChanged(int oldIndex, int newIndex)
    {
        SetActiveItem(newIndex);
    }

    void SetActiveItem(int index)
    {
        for (int i = 0; i < items.Count; i++)
        {
            bool active = (i == index);
            if (items[i] != null)
            {
                items[i].GetComponent<SyncActive>().SetActiv(active);
                RpcSetItemActive(items[i], active);
            }
        }
    }

    [Command]
    public void CmdActivateObject(GameObject syncIfStrah)
    {
        syncIfStrah.SetActive(true);
        if (syncIfStrah) NetworkServer.Spawn(syncIfStrah);
    }

    [ClientRpc]
    void RpcSetItemActive(GameObject item, bool active)
    {
        if (item != null)
            item.GetComponent<SyncActive>().SetActiv(active);
    }

    [Command]
    void CmdChangeItem(int newIndex)
    {
        currentItemIndex = newIndex;
    }
}