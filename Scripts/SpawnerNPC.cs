using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerNPC : MonoBehaviour
{
    public GameObject[] cubessssss;
    public int sugoma = 0;
    public GameObject slider;

    public void sugomamami(int newsugoma)
    {
        sugoma = newsugoma;
    }

    public void SpawnCUBEEEEEEBRA()
    {
        GameObject subebra = Instantiate(cubessssss[sugoma], new Vector3(Mathf.Round(Camera.main.transform.position.x), Mathf.Round(Camera.main.transform.position.y), Mathf.Round(Camera.main.transform.position.z)), Quaternion.identity);
        subebra.transform.parent = slider.transform;
        subebra.SetActive(true);
        Destroy(subebra.GetComponent<NetworkIdentity>());
    }
}
