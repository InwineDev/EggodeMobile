using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class fakelscript : MonoBehaviour
{
    public int fakeloid;
    public GameObject sv;

    private void Start()
    {
        StartCoroutine(EveryTickInArm());
    }

    private IEnumerator EveryTickInArm()
    {
        yield return new WaitForSeconds(1f);
        fakeloid++;
        if (fakeloid == 100)
        {
            print("spawned факельник");
            int chance = Random.Range(1, 100);
            if (chance >= 99)
            {
                GameObject sus = Instantiate(fakelink, transform.position + new Vector3(Random.Range(1, 150), transform.position.y, Random.Range(1, 150)), Quaternion.identity);
                sus.GetComponent<lookat>().target = gameObject;
                sv.SetActive(true);
            }
            fakeloid = 0;
        }
        StartCoroutine(EveryTickInArm());
    }

    public GameObject fakelink;

}
