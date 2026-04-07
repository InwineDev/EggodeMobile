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
        if (Camera.main != null)
            camera = Camera.main.transform;
        s = GetComponent<TipikalPredmet>();
        if (s != null)
            me = s.player;
    }

    private void Update()
    {
        if (!isOwned) return;
        if (s == null) s = GetComponent<TipikalPredmet>();
        if (s == null || s.usersettingitems == null || s.usersettingitems.player == null) return;
        if (s.usersettingitems.player.escaped) return;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public void MobileLeftMouseDownAction()
    {
        if (!isOwned) return;
        StartGrapple();
    }

    public void MobileLeftMouseUpAction()
    {
        if (!isOwned) return;
        if (s == null || s.itemdat == null || s.usersettingitems == null) return;

        s.itemdat.RemoveItems(1);
        if (s.itemdat.amount <= 0)
            s.usersettingitems.ChangeSkin(0);

        StopGrapple();
    }

    void StartGrapple()
    {
        if (camera == null)
        {
            if (Camera.main == null) return;
            camera = Camera.main.transform;
        }

        if (me == null && s != null)
            me = s.player;
        if (me == null) return;

        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxDistance))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            sprintJoint = me.AddComponent<SpringJoint>();
            sprintJoint.autoConfigureConnectedAnchor = false;
            sprintJoint.connectedAnchor = grapplePoint;

            sprintJoint.maxDistance = 4f;
            sprintJoint.minDistance = 4f;

            sprintJoint.spring = 19f;
            sprintJoint.damper = 15f;
            sprintJoint.massScale = 4.5f;

            if (line != null)
                line.positionCount = 2;

            CMDGrapple(hit.point);
        }
    }

   [Command]
    void CMDGrapple(Vector3 hit)
    {
        if (line != null)
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
        if (line != null)
            line.positionCount = 2;
        grapplePoint = hit;
    }

    void StopGrapple()
    {
        if (sprintJoint != null)
            Destroy(sprintJoint);
        isGrappling = false;
        if (line != null)
            line.positionCount = 0;
        CMDSTOPGrapple();
    }

    [Command]
    void CMDSTOPGrapple()
    {
        if (line != null)
            line.positionCount = 0;
        RPCSTOPGrapple();
    }

    [ClientRpc]
    void RPCSTOPGrapple()
    {
        if (line != null)
            line.positionCount = 0;
    }


    void DrawRope()
    {
        if (!sprintJoint || line == null || gunTip == null) return;

        line.SetPosition(0, gunTip.position);
        line.SetPosition(1, grapplePoint);
        CMDDrawRope(gunTip.position, grapplePoint);
    }

    [Command]
    void CMDDrawRope(Vector3 guntip, Vector3 gp)
    {
        if (line == null) return;
        line.SetPosition(0, guntip);
        line.SetPosition(1, gp);
        RPCDrawRope(guntip, gp);
    }

    [ClientRpc]
    void RPCDrawRope(Vector3 guntip, Vector3 gp)
    {
        if (line == null) return;
        line.SetPosition(0, guntip);
        line.SetPosition(1, gp);
    }
}
