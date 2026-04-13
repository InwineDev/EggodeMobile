using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Burst.CompilerServices;

public class GrapplingGun : NetworkBehaviour
{
    [SerializeField] private LineRenderer line;
    public Vector3 grapplePoint;
    [SerializeField] private Transform gunTip, camera;
    [SerializeField] private float maxDistance;
    [SerializeField] private SpringJoint sprintJoint;
    [SerializeField] private GameObject me;

    [SyncVar]
    public TipikalPredmet s;

    public bool isGrappling;
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
    private void OnEnable()
    {
        line = GetComponent<LineRenderer>();
        camera = Camera.main.transform;
        me = GetComponent<TipikalPredmet>().player;
    }

    private void Update()
    {
        if (!isOwned) return;
        if (s.usersettingitems.player.escaped) return;
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        } else if (Input.GetMouseButtonUp(0))
        {
            s.itemdat.RemoveItems(1);
            if (s.itemdat.amount <= 0)
            {
                s.usersettingitems.ChangeSkin(0);
            }
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxDistance))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            sprintJoint = me.AddComponent<SpringJoint>();
            sprintJoint.autoConfigureConnectedAnchor = false;
            sprintJoint.connectedAnchor = grapplePoint;

            //float dfp = Vector3.Distance(me.transform.position, grapplePoint);

            sprintJoint.maxDistance = 4f;
            sprintJoint.minDistance = 4f;

            sprintJoint.spring = 4.5f;
            sprintJoint.spring = 19f;
            sprintJoint.damper = 7f;
            sprintJoint.damper = 15f;
            sprintJoint.massScale = 4.5f;

            line.positionCount = 2;

            CMDGrapple(hit.point);
        }
    }

   [Command]
    void CMDGrapple(Vector3 hit)
    {
        line.positionCount = 2;
        grapplePoint = hit;
        RPCGrapple(hit);
    }


    private void OnDisable()
    {
        StopGrapple();
    }

    [ClientRpc]
    void RPCGrapple(Vector3 hit)
    {
        line.positionCount = 2;
        grapplePoint = hit;
    }

    void StopGrapple()
    {
        Destroy(sprintJoint);
        isGrappling = false;
        line.positionCount = 0;
        CMDSTOPGrapple();
    }

    [Command]
    void CMDSTOPGrapple()
    {
        line.positionCount = 0;
        RPCSTOPGrapple();
    }

    [ClientRpc]
    void RPCSTOPGrapple()
    {
        line.positionCount = 0;
    }


    void DrawRope()
    {
        if (!sprintJoint) return;

        line.SetPosition(0, gunTip.position);
        line.SetPosition(1, grapplePoint);
        CMDDrawRope(gunTip.position, grapplePoint);
    }

    [Command]
    void CMDDrawRope(Vector3 guntip, Vector3 gp)
    {
        line.SetPosition(0, guntip);
        line.SetPosition(1, gp);
        RPCDrawRope(guntip, gp);
    }

    [ClientRpc]
    void RPCDrawRope(Vector3 guntip, Vector3 gp)
    {
        line.SetPosition(0, guntip);
        line.SetPosition(1, gp);
    }
}
