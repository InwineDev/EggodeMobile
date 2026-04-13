using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class SpawnerBullet : NetworkBehaviour
{
    public GameObject mama;
    public AudioSource ad;
    public float kt = 1f;
    public bool ktbool;

    [Command]
    void CmdSpawn()
    {

        Vector3 ma = gameObject.transform.position;
        GameObject koshka = Instantiate(mama, ma, transform.rotation);
        NetworkServer.Spawn(koshka);
        ad.Play(0);
    }

    void Update()
    {
        if (isOwned && Input.GetMouseButtonDown(0))
        {
            if (ktbool == false)
            {
                CmdSpawn();
                StartCoroutine(kttime());
                ktbool = true;
            }
        }
    }

    private IEnumerator kttime()
    {
        yield return new WaitForSeconds(kt);
        ktbool = false;
    }

    private void OnEnable()
    {
        if (ktbool)
        {
            StartCoroutine(kttime());
        }
    }
}