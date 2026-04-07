using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

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

    [SyncVar]
    private bool noInited = true;
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

    [Header("Mobile UI Tags")]
    [SerializeField] private string interactButtonTag = "InteractButton";
    [SerializeField] private string spawnButtonTag = "SpawnButton";

    private Button interactButton;
    private Button spawnButton;
    private bool localInitComplete;
    private bool uiItemCreated;

    public override void OnStartClient()
    {
        base.OnStartClient();
        TryResolveReferences();
        EnsureInitialized();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        TryResolveReferences();
        EnsureInitialized();
    }

    public void init()
    {
        TryResolveReferences();
        EnsureInitialized();
    }

    private void TryResolveReferences()
    {
        if (mms == null)
            mms = GetComponent<MultiMusicSystem>();

        if (transform.parent != null && networkIdentity == null)
            networkIdentity = transform.parent.GetComponent<NetworkIdentity>();

        if (transform.parent != null && transform.parent.parent != null && animka == null)
            animka = transform.parent.parent.GetComponent<Animator>();

        if (usersettingitems == null)
        {
            userSettings[] allSettings = FindObjectsOfType<userSettings>(true);

            if (isOwned)
            {
                foreach (var settings in allSettings)
                {
                    if (settings != null && settings.isLocalPlayer)
                    {
                        usersettingitems = settings;
                        break;
                    }
                }
            }

            if (usersettingitems == null)
            {
                foreach (var settings in allSettings)
                {
                    if (settings == null) continue;
                    if (player != null && settings.player == player.GetComponent<FirstPersonController>())
                    {
                        usersettingitems = settings;
                        break;
                    }
                }
            }
        }

        if (player == null && usersettingitems != null && usersettingitems.player != null)
            player = usersettingitems.player.gameObject;
    }

    public void EnsureInitialized()
    {
        TryResolveReferences();

        if (localInitComplete && itemdat != null)
            return;

        if (isOwned && usersettingitems != null && usersettingitems.item != null && usersettingitems.slider != null && itemdat == null)
        {
            foreach (Transform child in usersettingitems.slider.transform)
            {
                itemdannie existing = child.GetComponent<itemdannie>();
                if (existing != null && existing.id == id)
                {
                    itemdat = existing;
                    existing.usersettingitems = usersettingitems;
                    uiItemCreated = true;
                    break;
                }
            }

            if (itemdat == null)
            {
                GameObject cat = Instantiate(usersettingitems.item, usersettingitems.slider.transform);
                itemdannie data = cat.GetComponent<itemdannie>();
                if (data != null)
                {
                    data.id = id;
                    data.usersettingitems = usersettingitems;
                    itemdat = data;
                    uiItemCreated = true;
                }
            }
        }

        if (itemdat != null)
            itemdat.Starting();

        localInitComplete = usersettingitems != null || itemdat != null || !isOwned;
        noInited = false;
    }

    private void Start()
    {
        TryResolveReferences();
        EnsureInitialized();
        FindButtonsByTag();
    }

    private void FindButtonsByTag()
    {
        if (interactButton == null)
        {
            GameObject interactObj = GameObject.FindGameObjectWithTag(interactButtonTag);
            if (interactObj != null)
                interactButton = interactObj.GetComponent<Button>();
        }

        if (spawnButton == null)
        {
            GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnButtonTag);
            if (spawnObj != null)
                spawnButton = spawnObj.GetComponent<Button>();
        }

        RebindButtons();
    }

    private void RebindButtons()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(MobileInteract);
            interactButton.onClick.AddListener(MobileInteract);
        }

        if (spawnButton != null)
        {
            spawnButton.onClick.RemoveListener(MobileSpawn);
            spawnButton.onClick.AddListener(MobileSpawn);
        }
    }

    private bool IsCurrentActiveItem()
    {
        if (!isOwned) return false;

        GameObject rootObj = transform.parent != null ? transform.parent.gameObject : gameObject;
        if (!rootObj.activeInHierarchy) return false;
        if (!gameObject.activeInHierarchy) return false;

        SyncActive syncActive = rootObj.GetComponent<SyncActive>();
        if (syncActive != null)
            return rootObj.activeSelf;

        return true;
    }

    void Update()
    {
        if (!isOwned)
        {
            if (networkIdentity != null && player != null)
            {
                NetworkIdentity playerNi = player.GetComponent<NetworkIdentity>();
                if (playerNi != null && playerNi.connectionToClient != null)
                {
                    try
                    {
                        networkIdentity.AssignClientAuthority(playerNi.connectionToClient);
                    }
                    catch { }
                }
            }
            return;
        }

        if (!localInitComplete || itemdat == null || usersettingitems == null)
            EnsureInitialized();

        if (interactButton == null || spawnButton == null)
            FindButtonsByTag();
    }

    public void MobileInteract()
    {
        if (!IsCurrentActiveItem()) return;
        if (player == null) return;

        FirstPersonController controller = player.GetComponent<FirstPersonController>();
        if (controller != null && controller.escaped) return;

        if (ktbool || !canDamage) return;

        if (animka != null)
            animka.Play(animationName);

        StartCoroutine(kttime());
        ktbool = true;

        Camera useCam = Camera.main;
        if (useCam == null && usersettingitems != null)
            useCam = usersettingitems.cam;
        if (useCam == null)
            return;

        Ray ray = new Ray(useCam.transform.position, useCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (mms != null)
                mms.PlayClip();

            Health hp = hit.transform.GetComponent<Health>();
            if (hp != null)
                DAMA3GE(hp);

            name24 itemHp = hit.transform.GetComponent<name24>();
            if (itemHp != null)
                DAMAGEITEM(itemHp);
        }
    }

    public void MobileSpawn()
    {
        if (!IsCurrentActiveItem()) return;
        if (!spawned) return;
        if (spawn == null) return;
        if (player == null) return;

        FirstPersonController controller = player.GetComponent<FirstPersonController>();
        if (controller != null && controller.escaped) return;

        if (!serverProperties.instance.spawnn) return;

        Camera useCam = Camera.main;
        if (useCam == null && usersettingitems != null)
            useCam = usersettingitems.cam;
        if (useCam == null)
            return;

        Ray ray = new Ray(useCam.transform.position, useCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Vector3 spawnPosition = hit.point + pogreh;
            CmdSpawnCat(spawnPosition);
        }
    }

    [Command]
    void DAMAGEITEM(name24 sus)
    {
        if (serverProperties.instance.destroy)
            sus._hp -= damage;
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        if (!serverProperties.instance.hp) return;

        sus.health -= damage;
        if (sus.health <= 0)
        {
            sus.health = 100;
            sus.hp.text = $"{sus.health} HP";
        }
    }

    private void OnEnable()
    {
        TryResolveReferences();
        EnsureInitialized();

        if (networkIdentity != null && player != null)
        {
            NetworkIdentity playerNi = player.GetComponent<NetworkIdentity>();
            if (playerNi != null && playerNi.connectionToClient != null)
            {
                try
                {
                    networkIdentity.AssignClientAuthority(playerNi.connectionToClient);
                }
                catch { }
            }
        }

        if (ktbool)
            StartCoroutine(kttime());
        else
            usersettingitems?.OnKtStart?.Invoke(0);

        FindButtonsByTag();
    }

    private void OnDisable()
    {
        if (interactButton != null)
            interactButton.onClick.RemoveListener(MobileInteract);

        if (spawnButton != null)
            spawnButton.onClick.RemoveListener(MobileSpawn);
    }

    private IEnumerator kttime()
    {
        usersettingitems?.OnKtStart?.Invoke(kt);
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

    [TargetRpc]
    void RpcSPAWNITEMS()
    {
        if (itemdat == null) return;

        itemdat.RemoveItems(1);
        if (itemdat.amount <= 0)
            usersettingitems?.ChangeSkin(0);
    }
}
