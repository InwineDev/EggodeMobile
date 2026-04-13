using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemLoader : NetworkBehaviour
{
    [SerializeField] private string[] path;
    [SerializeField] private GameObject error;

    [SerializeField] private TMP_Text strahErrora;

    public List<GameObject> objects = new List<GameObject>();

    private static bool strah;
    [SerializeField] private GameObject spookyscaryspunki;

    List<GameObject> loadBundles()
    {
        List<GameObject> strah = new List<GameObject>();
        foreach (GameObject item in FindObjectOfType<ItemLoaderLobby>().objects)
        {
            if (item.GetComponent<TipikalPredmet>() != null)
            {
                strah.Add(item);
                objects.Add(item);
            }
        }
        return strah;
    }


    void Start()
    {
        if (spookyscaryspunki == null)
        {
            Debug.LogError("Error: 'Ruki' GameObject not found as a child of " + gameObject.name);
            strahErrora.text = "Error: 'Ruki' GameObject not found as a child of " + gameObject.name;
            error.SetActive(true);
            return; // Stop execution to prevent further errors
        }
        if (!gameObject.GetComponent<NetworkIdentity>().isOwned)
        {
            return;
        }
        Invoke("loadBundle", 10f);
    }

    void loadBundle()
    {
        CMDitem();
    }

    [Command]
    void CMDitem()
    {
        foreach (var item in loadBundles())
        {
            var sus = Instantiate(item);
            sus.transform.localPosition = new Vector3(0.97f, 0.01899998f, -0.421f);
            if (sus != null)
            {
                NetworkServer.Spawn(sus, gameObject);
            }
            RPCitem(sus, spookyscaryspunki);
            sus.GetComponent<TipikalPredmet>().id = gameObject.transform.GetChild(0).GetChild(0).GetComponent<userSettings>().items.Count;
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<userSettings>().items.Add(sus);
            sus.GetComponent<TipikalPredmet>().init();
        }
    }

    [ClientRpc]
    void RPCitem(GameObject toRPC, GameObject kuda)
    {
        if (toRPC == null)
        {
            Debug.LogError("RPCitem: toRPC is null!");
            return;
        }

        if (kuda == null)
        {
            Debug.LogError("RPCitem: kuda is null!");
            kuda = gameObject.transform.Find("Ruki").gameObject;
        }
        if (kuda == null)
        {
            Debug.LogError("RPCitem: kuda is null!");
            return;
        }

        toRPC.transform.parent = kuda.transform;
    }


}
