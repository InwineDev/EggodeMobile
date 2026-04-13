using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingController : NetworkBehaviour
{
    [SyncVar]
    public int sittingPlayers = 0;

    public Vector3[] sittingPosition;
}
