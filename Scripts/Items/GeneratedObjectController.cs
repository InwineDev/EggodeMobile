using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedObjectController : NetworkBehaviour
{
    [SyncVar(hook = nameof(SyncTexture))]
    public int textureSeed;

    private void SyncTexture(int old, int neww)
    {
        textureSeed = neww;
    }
}
