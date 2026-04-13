using Mirror;
using UnityEngine;

public class SyncParent : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnParentChanged))]
    private Transform parentIdentity;

    [Server]
    public void SetParent(Transform parent)
    {
        parentIdentity = parent;
        transform.SetParent(parent.transform);
    }

    private void OnParentChanged(Transform oldParent, Transform newParent)
    {
        if (newParent != null)
            transform.SetParent(newParent.transform);
    }
}