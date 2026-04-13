using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallHammerController : NetworkBehaviour
{
    private FirstPersonController player;
    private TipikalPredmet tp;
    [SerializeField] private float raycastDistance;
    private Vector3 spawnPosition;
    private GameObject spawnedGhost;
    [SerializeField] private GameObject GhostPrefab;
    [SerializeField] private GameObject CubePrefab;
    void Start()
    {
        tp = GetComponent<TipikalPredmet>();
        player = tp.player.GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (!isOwned) return;
        if (Input.GetMouseButtonDown(1))
        {
            if (!player.escaped)
            {
                if (serverProperties.instance.spawnn)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    /*RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, raycastDistance))
                    {*/
                    spawnPosition = GetRaycastHitPoint(ray, raycastDistance);
                    spawnedGhost = Instantiate(GhostPrefab, spawnPosition, Quaternion.identity);
                    //}
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (spawnPosition != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit hit;
                //Physics.Raycast(ray, out hit, raycastDistance);
                //{
                    Vector3 localSpawnPosition = GetRaycastHitPoint(ray, raycastDistance);
                    spawnedGhost.transform.localScale = (localSpawnPosition - spawnPosition) * 50;
                //}
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, raycastDistance))
            //{
                Vector3 localSpawnPosition = GetRaycastHitPoint(ray, raycastDistance);
                Destroy(spawnedGhost);
                spawnedGhost = null;
                GameObject spawnedCube = Instantiate(CubePrefab, spawnPosition, Quaternion.identity);
                NetworkServer.Spawn(spawnedCube);

                spawnedCube.transform.localScale = (localSpawnPosition - spawnPosition) * 50;

                spawnPosition = new Vector3(0,0,0);
            //}
        }
    }
    Vector3 GetRaycastHitPoint(Ray ray, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            return hit.point;
        }
        else
        {
            return ray.origin + ray.direction * distance;
        }
    }
}
