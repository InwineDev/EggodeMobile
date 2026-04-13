using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IIScript : MonoBehaviour
{
    public float speed = 5f;
    public float stoppingDistance = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(player.transform);
        }
    }
}
