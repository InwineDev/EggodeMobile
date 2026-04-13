using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEnable : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(strah());
    }

    private IEnumerator strah()
    {
        yield return new WaitForSeconds(4f);
        gameObject.GetComponent<CsModsManager>().enabled = true;
    }
}
