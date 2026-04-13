using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarDictionary : NetworkBehaviour
{
    public Dictionary<string, object> values = new Dictionary<string, object>();
}
