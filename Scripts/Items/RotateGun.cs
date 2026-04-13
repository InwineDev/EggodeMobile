using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RotateGun : NetworkBehaviour
{
    public GrapplingGun grappler;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

/*    private void Update()
    {
        if (!isLocalPlayer) return;
        if (!grappler.isGrappling)
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappler.GetGrapplePoint() - transform.position);
        }
        transform.LookAt((grappler.grapplePoint));
        CMDLookAt(grappler.grapplePoint);
    }*/
    private void Update()
    {
        if (!grappler.isGrappling)
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappler.GetGrapplePoint() - transform.position);
        }

        CMDLookAt(desiredRotation, rotationSpeed);
    }

    [Command]
    void CMDLookAt(Quaternion desiredRotat, float rotatSpeed)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotat, Time.deltaTime * rotatSpeed);
        RPCLookAt(desiredRotat, rotatSpeed);
    }

    [ClientRpc]
    void RPCLookAt(Quaternion desiredRotat, float rotatSpeed)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotat, Time.deltaTime * rotatSpeed);
    }

}
