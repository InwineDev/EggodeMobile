using Mirror;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class userSettings : NetworkBehaviour
{
    public Camera cam;
    public AudioListener cam1;
    public GameObject skin;
    public GameObject item;
    public GameObject nickname;
    public GameObject canvas;

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

    [Header("Optional Mobile UI Buttons")]
    public Button interactButton;
    public Button destroyButton;
    public Button fButton;
    public Button leftMouseButton;
    public Button rightMouseButton;
    public Button dropButton;
    public Button toggleF1Button;
    public Button nextItemButton;
    public Button prevItemButton;

    [Header("Mobile Flags")]
    [SerializeField] private bool mobileUsePressed;
    [SerializeField] private bool mobileDestroyPressed;
    [SerializeField] private bool mobileDropPressed;
    [SerializeField] private bool mobileToggleF1Pressed;

    [SerializeField] private bool mobileLeftMouseDownPressed;
    [SerializeField] private bool mobileLeftMouseHeld;
    [SerializeField] private bool mobileLeftMouseUpPressed;

    [SerializeField] private bool mobileRightMouseDownPressed;
    [SerializeField] private bool mobileRightMouseHeld;
    [SerializeField] private bool mobileRightMouseUpPressed;

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
        if (ktController != null)
            OnKtStart += ktController.PlayCenterAnimation;
    }

    private void OnDisable()
    {
        if (ktController != null)
            OnKtStart -= ktController.PlayCenterAnimation;

        UnbindMobileButtons();
    }

    public void AddItem(int id)
    {
        AddItem(id, 1);
    }

    public void AddItem(int id, int amount)
    {
        if (id < 0 || id >= idannie.Count) return;
        if (idannie[id] == null) return;
        if (idannie[id].itemdat == null) return;

        idannie[id].itemdat.amount += amount;
    }

    public void OnWormChanged(int oldv, int newv)
    {
        if (newv >= 98 && wormObject != null)
        {
            wormObject.SetActive(true);
        }
    }

    void Start()
    {
        if (isServer)
            spawnitemmenu();

        StartCoroutine(OMG());

        if (!isLocalPlayer)
        {
            if (cam != null) cam.enabled = false;
            if (cam1 != null) cam1.enabled = false;
            if (canvas != null) canvas.SetActive(false);
            if (skin != null) skin.SetActive(true);
        }
        else
        {
            if (cam != null) cam.enabled = true;
            if (cam1 != null) cam1.enabled = true;
            if (skin != null) SetLayerToChildren(skin, 7);
            if (canvas != null) canvas.SetActive(true);
            worm = UnityEngine.Random.Range(1, 50);

            BindMobileButtons();
        }
    }

    void SetLayerToChildren(GameObject parent, int newLayer)
    {
        if (parent == null) return;

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
            if (hptxt != null) hptxt.SetActive(false);
            if (nametxt != null) nametxt.SetActive(false);
        }
        else
        {
            if (hptxt != null) hptxt.SetActive(true);
            if (nametxt != null) nametxt.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcAddItemToList(GameObject itemObj)
    {
        if (!isServer && isLocalPlayer && itemObj != null && !items.Contains(itemObj))
        {
            items.Add(itemObj);
        }
    }

    [Server]
    public void spawnitemmenu()
    {
        for (int i = 0; i < itemsPrefabs.Count; i++)
        {
            GameObject itemObj = Instantiate(itemsPrefabs[i]);
            NetworkServer.Spawn(itemObj, connectionToClient);

            NetworkIdentity ni = itemObj.GetComponent<NetworkIdentity>();
            if (ni != null)
            {
                try
                {
                    ni.AssignClientAuthority(connectionToClient);
                }
                catch { }
            }

            SyncActive syncActive = itemObj.GetComponent<SyncActive>();
            if (syncActive == null || syncActive.tpk == null)
                continue;

            TipikalPredmet tp = syncActive.tpk;
            tp.id = i;
            tp.usersettingitems = this;
            tp.player = player != null ? player.gameObject : null;

            idannie.Add(tp);
            items.Add(itemObj);
            StartCoroutine(InitItem(itemObj, i));
        }

        for (int i = 0; i < ModLoader.instance.objectsItems.Count; i++)
        {
            try
            {
                GameObject itemObj = Instantiate(ModLoader.instance.objectsItems[i].itemInArm);
                NetworkServer.Spawn(itemObj, connectionToClient);

                NetworkIdentity ni = itemObj.GetComponent<NetworkIdentity>();
                if (ni != null)
                {
                    try
                    {
                        ni.AssignClientAuthority(connectionToClient);
                    }
                    catch { }
                }

                int id = itemsPrefabs.Count + i;

                SyncActive syncActive = itemObj.GetComponent<SyncActive>();
                if (syncActive == null || syncActive.tpk == null)
                    continue;

                TipikalPredmet tp = syncActive.tpk;
                tp.id = id;
                tp.usersettingitems = this;
                tp.player = player != null ? player.gameObject : null;
                tp.spawn = ModLoader.instance.objectsItems[i].spawnedItem;

                items.Add(itemObj);
                idannie.Add(tp);

                StartCoroutine(InitItem(itemObj, id));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [Server]
    IEnumerator InitItem(GameObject itemObj, int index)
    {
        yield return new WaitForSeconds(0.1f);

        if (itemObj == null || ruki == null)
            yield break;

        itemObj.transform.SetParent(ruki.transform);
        itemObj.transform.localPosition = Vector3.zero;
        itemObj.transform.localRotation = Quaternion.identity;

        SyncActive syncActive = itemObj.GetComponent<SyncActive>();
        if (syncActive == null || syncActive.tpk == null)
            yield break;

        TipikalPredmet tp = syncActive.tpk;
        tp.player = player != null ? player.gameObject : null;
        tp.usersettingitems = this;
        tp.id = index;
        tp.init();

        syncActive.SetActiv(index == 0);
        currentItemIndex = 0;

        RpcInitItem(itemObj, index, index == 0);
    }

    [ClientRpc]
    void RpcInitItem(GameObject itemObj, int i, bool active)
    {
        if (isServer) return;
        if (itemObj == null) return;
        if (ruki == null) return;

        SyncActive syncAct = itemObj.GetComponent<SyncActive>();
        if (syncAct == null || syncAct.tpk == null) return;

        itemObj.transform.SetParent(ruki.transform);
        itemObj.transform.localPosition = Vector3.zero;
        itemObj.transform.localRotation = Quaternion.identity;

        TipikalPredmet tp = syncAct.tpk;
        tp.usersettingitems = this;
        tp.player = player != null ? player.gameObject : null;
        tp.id = i;
        tp.init();

        syncAct.SetActiv(active);

        if (!items.Contains(itemObj))
            items.Add(itemObj);
    }

    public void ClearChest(GameObject item1)
    {
        chestItemList.Remove(item1);
        if (item1 != null)
            Destroy(item1);
    }

    public void ClearAllChest()
    {
        List<GameObject> itemsToDestroy = new List<GameObject>(chestItemList);
        chestItemList.Clear();

        foreach (var itemObj in itemsToDestroy)
        {
            if (itemObj != null)
                Destroy(itemObj);
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

        NetworkIdentity ni = sitting.GetComponent<NetworkIdentity>();
        if (ni != null && ni.connectionToClient != null)
        {
            try
            {
                ni.RemoveClientAuthority();
            }
            catch { }
        }

        sitting = null;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (sitting != null)
            player.transform.localPosition = sitting.sittingPosition[sittingNumber];

        if (canWrite)
        {
            ResetMobileButtons();
            return;
        }

        if (mobileToggleF1Pressed)
            HandleToggleF1();

        if (mobileDropPressed)
            HandleDropCurrentItem();

        if (mobileDestroyPressed)
            HandleDestroyAction();

        if (mobileUsePressed)
            HandleUseAction();

        if (mobileLeftMouseDownPressed)
            HandleLeftMouseDownAction();

        if (mobileLeftMouseHeld)
            HandleLeftMouseHoldAction();

        if (mobileLeftMouseUpPressed)
            HandleLeftMouseUpAction();

        if (mobileRightMouseDownPressed)
            HandleRightMouseDownAction();

        if (mobileRightMouseHeld)
            HandleRightMouseHoldAction();

        if (mobileRightMouseUpPressed)
            HandleRightMouseUpAction();

        ResetMobileButtons();
    }

    private Camera GetUseCamera()
    {
        if (cam != null) return cam;
        if (Camera.main != null) return Camera.main;
        return null;
    }

    private void HandleToggleF1()
    {
        if (f1 != null)
            f1.SetActive(!f1.activeSelf);
    }

    private void HandleDropCurrentItem()
    {
        if (droppedPrefab == null) return;
        CmdDropCurrentItem(currentItemIndex, transform.position, Quaternion.identity);
    }

    private void HandleDestroyAction()
    {
        if (serverProperties.instance == null || !serverProperties.instance.destroy)
            return;

        Camera useCam = GetUseCamera();
        if (useCam == null) return;

        RaycastHit hit;
        if (Physics.Raycast(useCam.transform.position, useCam.transform.forward, out hit, raycastDistance))
        {
            GameObject hitObject = hit.collider != null ? hit.collider.gameObject : null;
            if (hitObject != null && hitObject.CompareTag("object"))
            {
                CmdDestroyObject(hitObject);
            }
        }
    }

    private void HandleLeftMouseDownAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileLeftMouseDownAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleLeftMouseHoldAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileLeftMouseHoldAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleLeftMouseUpAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileLeftMouseUpAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleRightMouseDownAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileRightMouseDownAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleRightMouseHoldAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileRightMouseHoldAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleRightMouseUpAction()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileRightMouseUpAction", SendMessageOptions.DontRequireReceiver);
    }

    private void HandleUseAction()
    {
        Camera useCam = GetUseCamera();
        if (useCam == null) return;

        Vector3 origin = useCam.transform.position;
        Vector3 direction = useCam.transform.forward;

        if (sitting != null)
        {
            StandFromVzaim();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, raycastDistance))
        {
            GameObject hitObject = hit.collider != null ? hit.collider.gameObject : null;
            if (hitObject == null) return;

            SittingController vzObject = hitObject.GetComponent<SittingController>();
            if (vzObject != null && vzObject.sittingPlayers <= vzObject.sittingPosition.Length)
            {
                sitting = vzObject;
                sitting.tag = "Untagged";
                player.transform.SetParent(hitObject.transform);
                sittingNumber = vzObject.sittingPlayers;
                player.transform.localPosition = sitting.sittingPosition[sittingNumber];

                if (vzObject.sittingPlayers == 0)
                    AssignSitting(sitting.GetComponent<NetworkIdentity>());

                vzObject.sittingPlayers++;
                ChangeRigidAndTrigger();
                return;
            }

            scriptor sc = hitObject.GetComponent<scriptor>();
            if (sc != null)
            {
                CallInteractObject(sc);
            }

            interactable inter = hitObject.GetComponent<interactable>();
            if (inter != null)
            {
                inter.interact(player);
            }
        }
    }

    private void ResetMobileButtons()
    {
        mobileUsePressed = false;
        mobileDestroyPressed = false;
        mobileDropPressed = false;
        mobileToggleF1Pressed = false;

        mobileLeftMouseDownPressed = false;
        mobileLeftMouseUpPressed = false;

        mobileRightMouseDownPressed = false;
        mobileRightMouseUpPressed = false;
    }

    [Command]
    void ChangeRigidAndTrigger()
    {
        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        Collider col = player.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = !rb.isKinematic;
        if (col != null) col.isTrigger = !col.isTrigger;
    }

    [ClientRpc]
    void ChangeRpcRigidAndTrigger()
    {
        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        Collider col = player.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = !rb.isKinematic;
        if (col != null) col.isTrigger = !col.isTrigger;
    }

    [Command]
    void AssignSitting(NetworkIdentity neti)
    {
        if (neti == null) return;

        try
        {
            neti.AssignClientAuthority(connectionToClient);
        }
        catch { }
    }

    [Command]
    void CallInteractObject(scriptor s)
    {
        CallInteractOnClients(s);
    }

    [ClientRpc]
    void CallInteractOnClients(scriptor s)
    {
        if (!isOwned || s == null) return;

        if (s.type == 1)
            s.typeController(Player228);
    }

    public void OnEnterNpc()
    {
        if (player != null)
            player.escaped = false;

        if (npcOkno != null && npcOkno.Length > 0 && npcOkno[0] != null)
            npcOkno[0].SetActive(false);
    }

    void IDEM1(SittingController o, SittingController d)
    {
        RPCSUS(d);
    }

    [TargetRpc]
    void RPCSUS(SittingController d)
    {
        if (d != null && Player228 != null)
            Player228.transform.parent = d.transform;
    }

    [SyncVar]
    public SittingController sitting;

    public GameObject Player228;

    [Command]
    void CmdDestroyObject(GameObject objToDestroy)
    {
        if (objToDestroy == null) return;
        NetworkServer.Destroy(objToDestroy);
    }

    [Command]
    void CmdDropCurrentItem(int itemIndex, Vector3 spawnPos, Quaternion spawnRot)
    {
        if (droppedPrefab == null) return;

        GameObject dropped = Instantiate(droppedPrefab, spawnPos, spawnRot);
        DroppedObjectController droppedCtrl = dropped.GetComponent<DroppedObjectController>();
        if (droppedCtrl != null)
            droppedCtrl.id = itemIndex;

        NetworkServer.Spawn(dropped);
    }

    [Command]
    void CmdSpawnKoshka(int prefabToSpawn, Vector3 hit)
    {
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
        if (newSkinIndex < 0 || newSkinIndex >= items.Count) return;
        if (items[newSkinIndex] == null) return;

        SyncActive syncActive = items[newSkinIndex].GetComponent<SyncActive>();
        if (syncActive == null || syncActive.tpk == null || syncActive.tpk.itemdat == null) return;

        if (syncActive.tpk.itemdat.amount <= 0 && newSkinIndex != 0) return;

        CmdChangeSkin(newSkinIndex);
        OnChangeItem?.Invoke(newSkinIndex);
    }

    public void PunchPlay()
    {
        if (punchAudio != null)
            punchAudio.PlayClip();
    }

    [Command]
    private void CmdChangeSkin(int newSkinIndex)
    {
        currentItemIndex = newSkinIndex;
        if (changeItem != null)
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
                SyncActive sa = items[i].GetComponent<SyncActive>();
                if (sa != null)
                    sa.SetActiv(active);

                RpcSetItemActive(items[i], active);
            }
        }
    }

    [Command]
    public void CmdActivateObject(GameObject syncIfStrah)
    {
        if (syncIfStrah == null) return;

        syncIfStrah.SetActive(true);
        NetworkServer.Spawn(syncIfStrah);
    }

    [ClientRpc]
    void RpcSetItemActive(GameObject itemObj, bool active)
    {
        if (itemObj != null)
        {
            SyncActive sa = itemObj.GetComponent<SyncActive>();
            if (sa != null)
                sa.SetActiv(active);
        }
    }

    [Command]
    void CmdChangeItem(int newIndex)
    {
        currentItemIndex = newIndex;
    }

    private List<int> GetAvailableItemIndexes()
    {
        List<int> availableIndexes = new List<int>();

        foreach (var itemObj in idannie)
        {
            if (itemObj != null && itemObj.itemdat != null && itemObj.transform.parent != null)
            {
                if (itemObj.itemdat.amount > 0)
                {
                    int idx = items.IndexOf(itemObj.transform.parent.gameObject);
                    if (idx >= 0)
                        availableIndexes.Add(idx);
                }
            }
        }

        availableIndexes.Sort();
        return availableIndexes;
    }

    public void MobileNextItem()
    {
        if (!isLocalPlayer) return;

        List<int> availableIndexes = GetAvailableItemIndexes();
        if (availableIndexes.Count == 0) return;

        int nextIndex = -1;
        for (int i = 0; i < availableIndexes.Count; i++)
        {
            if (availableIndexes[i] > currentItemIndex)
            {
                nextIndex = availableIndexes[i];
                break;
            }
        }

        if (nextIndex == -1)
            nextIndex = availableIndexes[0];

        ChangeSkin(nextIndex);
    }

    public void MobilePrevItem()
    {
        if (!isLocalPlayer) return;

        List<int> availableIndexes = GetAvailableItemIndexes();
        if (availableIndexes.Count == 0) return;

        int prevIndex = -1;
        for (int i = availableIndexes.Count - 1; i >= 0; i--)
        {
            if (availableIndexes[i] < currentItemIndex)
            {
                prevIndex = availableIndexes[i];
                break;
            }
        }

        if (prevIndex == -1)
            prevIndex = availableIndexes[availableIndexes.Count - 1];

        ChangeSkin(prevIndex);
    }

    public void MobileInteract()
    {
        if (!isLocalPlayer) return;
        mobileUsePressed = true;
    }

    public void MobileDestroy()
    {
        if (!isLocalPlayer) return;
        mobileDestroyPressed = true;
    }

    public void MobileDropCurrentItem()
    {
        if (!isLocalPlayer) return;
        mobileDropPressed = true;
    }

    public void MobileToggleF1()
    {
        if (!isLocalPlayer) return;
        mobileToggleF1Pressed = true;
    }

    public void MobileUseDirect()
    {
        if (!isLocalPlayer) return;
        HandleUseAction();
    }

    public void MobileDestroyDirect()
    {
        if (!isLocalPlayer) return;
        HandleDestroyAction();
    }

    public void MobileFDirect()
    {
        if (!isLocalPlayer) return;

        if (currentItemIndex < 0 || currentItemIndex >= items.Count) return;
        if (items[currentItemIndex] == null) return;

        items[currentItemIndex].BroadcastMessage("MobileFAction", SendMessageOptions.DontRequireReceiver);
    }

    public void MobileLeftMouseDirect()
    {
        if (!isLocalPlayer) return;
        HandleLeftMouseDownAction();
    }

    public void MobileLeftMousePointerDown()
    {
        if (!isLocalPlayer) return;
        mobileLeftMouseHeld = true;
        mobileLeftMouseDownPressed = true;
    }

    public void MobileLeftMousePointerUp()
    {
        if (!isLocalPlayer) return;
        mobileLeftMouseHeld = false;
        mobileLeftMouseUpPressed = true;
    }

    public void MobileRightMouseDirect()
    {
        if (!isLocalPlayer) return;
        HandleRightMouseDownAction();
    }

    public void MobileRightMousePointerDown()
    {
        if (!isLocalPlayer) return;
        mobileRightMouseHeld = true;
        mobileRightMouseDownPressed = true;
    }

    public void MobileRightMousePointerUp()
    {
        if (!isLocalPlayer) return;
        mobileRightMouseHeld = false;
        mobileRightMouseUpPressed = true;
    }

    public void MobileDropDirect()
    {
        if (!isLocalPlayer) return;
        HandleDropCurrentItem();
    }

    public void MobileToggleF1Direct()
    {
        if (!isLocalPlayer) return;
        HandleToggleF1();
    }

    private void BindMobileButtons()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(MobileUseDirect);
            interactButton.onClick.AddListener(MobileUseDirect);
        }

        if (destroyButton != null)
        {
            destroyButton.onClick.RemoveListener(MobileDestroyDirect);
            destroyButton.onClick.AddListener(MobileDestroyDirect);
        }

        if (fButton != null)
        {
            fButton.onClick.RemoveListener(MobileFDirect);
            fButton.onClick.AddListener(MobileFDirect);
        }

        if (leftMouseButton != null)
        {
            leftMouseButton.onClick.RemoveListener(MobileLeftMouseDirect);
            leftMouseButton.onClick.AddListener(MobileLeftMouseDirect);
        }

        if (rightMouseButton != null)
        {
            rightMouseButton.onClick.RemoveListener(MobileRightMouseDirect);
            rightMouseButton.onClick.AddListener(MobileRightMouseDirect);
        }

        if (dropButton != null)
        {
            dropButton.onClick.RemoveListener(MobileDropDirect);
            dropButton.onClick.AddListener(MobileDropDirect);
        }

        if (toggleF1Button != null)
        {
            toggleF1Button.onClick.RemoveListener(MobileToggleF1Direct);
            toggleF1Button.onClick.AddListener(MobileToggleF1Direct);
        }

        if (nextItemButton != null)
        {
            nextItemButton.onClick.RemoveListener(MobileNextItem);
            nextItemButton.onClick.AddListener(MobileNextItem);
        }

        if (prevItemButton != null)
        {
            prevItemButton.onClick.RemoveListener(MobilePrevItem);
            prevItemButton.onClick.AddListener(MobilePrevItem);
        }
    }

    private void UnbindMobileButtons()
    {
        if (interactButton != null)
            interactButton.onClick.RemoveListener(MobileUseDirect);

        if (destroyButton != null)
            destroyButton.onClick.RemoveListener(MobileDestroyDirect);

        if (fButton != null)
            fButton.onClick.RemoveListener(MobileFDirect);

        if (leftMouseButton != null)
            leftMouseButton.onClick.RemoveListener(MobileLeftMouseDirect);

        if (rightMouseButton != null)
            rightMouseButton.onClick.RemoveListener(MobileRightMouseDirect);

        if (dropButton != null)
            dropButton.onClick.RemoveListener(MobileDropDirect);

        if (toggleF1Button != null)
            toggleF1Button.onClick.RemoveListener(MobileToggleF1Direct);

        if (nextItemButton != null)
            nextItemButton.onClick.RemoveListener(MobileNextItem);

        if (prevItemButton != null)
            prevItemButton.onClick.RemoveListener(MobilePrevItem);
    }
}