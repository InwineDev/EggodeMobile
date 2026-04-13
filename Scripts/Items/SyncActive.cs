using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncActive : MonoBehaviour
{
    public TipikalPredmet tpk;

/*    private void Start()
    {
        tpk = obj.GetComponent<TipikalPredmet>();
    }
*/
    public void SetActiv(bool active)
    {
        tpk.gameObject.SetActive(active);
    }
}
