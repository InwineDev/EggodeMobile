using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DependenceController : MonoBehaviour
{
    public TMP_InputField tmpif;
    public ModsDependences mD;

    public void DestroyMe()
    {
        mD.dcs.Remove(this);
        Destroy(gameObject);
    }
}
