using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemId : NetworkBehaviour
{
    [SyncVar]
    public int id;
}
