using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class serversettings : NetworkBehaviour
{
    
    public Toggle toghp;
    public Toggle togspawn;
    public Toggle togdestroy;
    [SerializeField] private GameObject prefabPlayers;
    [SerializeField] private GameObject slider;

    public void onhptogchange()
    {
        FindObjectOfType<serverProperties>().hp = toghp.isOn;
    }

    public virtual void OnClientConnect()
    {
        GameObject s = Instantiate(prefabPlayers);

        s.transform.parent = slider.transform;
        NetworkServer.Spawn(s);
    }
    public void onspawnntogchange()
    {
        FindObjectOfType<serverProperties>().spawnn = togspawn.isOn;
    }

    public void ondestroytogchange()
    {
        FindObjectOfType<serverProperties>().destroy = togdestroy.isOn;
    }
}
