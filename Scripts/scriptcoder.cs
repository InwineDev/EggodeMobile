using Mirror;
using UnityEngine;

public class scriptcoder : NetworkBehaviour
{
    [SyncVar]
    public Vector3 tp;

    [SyncVar]
    public bool tpbool;

    private void OnTriggerEnter(Collider other)
    {
        if(tpbool)
        {
            Vector3 pon = gameObject.transform.position - other.transform.position;
            other.transform.position = tp - pon;
        }
    }
}