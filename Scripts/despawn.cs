using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despawn : MonoBehaviour
{
    public float sus;
    void Start()
    {
        StartCoroutine(pora());
    }

    public IEnumerator pora()
    {
        yield return new WaitForSeconds(sus);
        Destroy(gameObject);
    }
}
