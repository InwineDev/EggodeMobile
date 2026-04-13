using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.Burst.CompilerServices;

public class portalscript : NetworkBehaviour
{
    [System.Serializable]
    public struct PortalData
    {
        public GameObject predmeti;
        public GameObject newportal;
    }
    public PortalData[] portalData;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (PortalData poppy in portalData)
        {
            string name228;
            if (collision.gameObject.GetComponent<name24>())
            {
                name228 = collision.gameObject.GetComponent<name24>().name244;
            } else
            {
                name228 = "Вонючий сурикат";
            }
            if (poppy.predmeti.GetComponent<name24>().name244 == name228) {
                Destroy(collision.gameObject);
                GameObject cat = Instantiate(poppy.newportal, gameObject.transform.position, gameObject.transform.rotation);
                NetworkServer.Spawn(cat, connectionToClient);
                NetworkServer.Destroy(gameObject);
            }
        }
    }

}
