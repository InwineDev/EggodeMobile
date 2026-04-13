using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RainEvent : NetworkBehaviour
{
    public RandomEventObject Event;
    public GameObject rained;
    public Transform[] points;

    private void OnEnable() => Event.OnEventRaised.AddListener(rain);
    private void OnDisable() => Event.OnEventRaised.RemoveListener(rain);

    [Server]
    void rain()
    {
        StartCoroutine(rainCoroutine());
    }

    IEnumerator rainCoroutine()
    {
        for (int i = 0; i < Random.Range(1,1000); i++)
        {
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            Vector3 spawn = new Vector3(Random.Range(points[0].position.x, points[1].position.x), points[0].position.y, Random.Range(points[0].position.z, points[1].position.z));
            GameObject s = Instantiate(rained, spawn, Quaternion.identity);
            NetworkServer.Spawn(s);
        }
    }
}
