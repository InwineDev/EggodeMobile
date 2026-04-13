using Mirror;
using UnityEngine;

public class DroppedObjectController : NetworkBehaviour
{
    [SyncVar]
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<userSettingNotCam>().us.AddItem(id);
        NetworkServer.Destroy(gameObject);
    }
}
