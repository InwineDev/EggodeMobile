using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemLoader : NetworkBehaviour
{
    [SerializeField] private string[] path;
    [SerializeField] private GameObject error;
    [SerializeField] private TMP_Text strahErrora;

    public List<GameObject> objects = new List<GameObject>();

    [SerializeField] private GameObject spookyscaryspunki;

    private bool localLoadStarted;
    [SyncVar] private bool serverItemsInitialized;

    List<GameObject> LoadBundles()
    {
        List<GameObject> loadedItems = new List<GameObject>();
        ItemLoaderLobby lobbyLoader = FindObjectOfType<ItemLoaderLobby>();
        if (lobbyLoader == null)
            return loadedItems;

        foreach (GameObject item in lobbyLoader.objects)
        {
            if (item == null || item.GetComponent<TipikalPredmet>() == null)
                continue;

            if (!loadedItems.Contains(item))
                loadedItems.Add(item);

            if (!objects.Contains(item))
                objects.Add(item);
        }

        return loadedItems;
    }

    void Start()
    {
        if (spookyscaryspunki == null)
        {
            Debug.LogError("Error: 'Ruki' GameObject not found as a child of " + gameObject.name);
            if (strahErrora != null)
                strahErrora.text = "Error: 'Ruki' GameObject not found as a child of " + gameObject.name;
            if (error != null)
                error.SetActive(true);
            return;
        }

        if (!isOwned)
            return;

        StartCoroutine(LoadBundleWhenReady());
    }

    private IEnumerator LoadBundleWhenReady()
    {
        if (localLoadStarted)
            yield break;

        localLoadStarted = true;

        ItemLoaderLobby lobbyLoader = null;
        float timeout = Time.time + 15f;

        while (Time.time < timeout)
        {
            lobbyLoader = FindObjectOfType<ItemLoaderLobby>();
            if (lobbyLoader != null && lobbyLoader.IsLoaded)
                break;

            yield return null;
        }

        CMDitem();
    }

    [Command]
    void CMDitem()
    {
        if (serverItemsInitialized)
            return;

        userSettings settings = gameObject.transform.GetChild(0).GetChild(0).GetComponent<userSettings>();
        if (settings == null)
            return;

        foreach (var item in LoadBundles())
        {
            if (item == null)
                continue;

            GameObject spawnedItem = Instantiate(item);
            spawnedItem.transform.SetParent(spookyscaryspunki.transform, false);
            spawnedItem.transform.localPosition = new Vector3(0.97f, 0.01899998f, -0.421f);
            spawnedItem.transform.localRotation = Quaternion.identity;

            SyncActive syncActive = spawnedItem.GetComponent<SyncActive>();
            TipikalPredmet tipikalPredmet = spawnedItem.GetComponent<TipikalPredmet>();
            if (syncActive == null || syncActive.tpk == null || tipikalPredmet == null)
            {
                Destroy(spawnedItem);
                continue;
            }

            syncActive.SetActiv(false);
            tipikalPredmet.id = settings.items.Count;
            tipikalPredmet.usersettingitems = settings;
            tipikalPredmet.player = settings.player != null ? settings.player.gameObject : null;

            NetworkServer.Spawn(spawnedItem, connectionToClient);
            settings.items.Add(spawnedItem);

            tipikalPredmet.init();
            RPCitem(spawnedItem, spookyscaryspunki, tipikalPredmet.id);
        }

        serverItemsInitialized = true;
    }

    [ClientRpc]
    void RPCitem(GameObject toRPC, GameObject kuda, int itemId)
    {
        if (toRPC == null)
        {
            Debug.LogError("RPCitem: toRPC is null!");
            return;
        }

        if (kuda == null)
            kuda = gameObject.transform.Find("Ruki")?.gameObject;

        if (kuda == null)
        {
            Debug.LogError("RPCitem: kuda is null!");
            return;
        }

        toRPC.transform.SetParent(kuda.transform, false);
        toRPC.transform.localPosition = new Vector3(0.97f, 0.01899998f, -0.421f);
        toRPC.transform.localRotation = Quaternion.identity;

        TipikalPredmet tipikalPredmet = toRPC.GetComponent<TipikalPredmet>();
        if (tipikalPredmet != null)
        {
            tipikalPredmet.id = itemId;
            tipikalPredmet.init();
        }

        SyncActive syncActive = toRPC.GetComponent<SyncActive>();
        if (syncActive != null)
            syncActive.SetActiv(false);
    }
}
