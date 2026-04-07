using UnityEngine;
using Mirror;

public class GrapplingGun : NetworkBehaviour
{
    [SerializeField] private LineRenderer line;
    public Vector3 grapplePoint;
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform camera;
    [SerializeField] private float maxDistance;
    [SerializeField] private SpringJoint sprintJoint;
    [SerializeField] private GameObject me;
    [SerializeField] private string releaseButtonTag = "ReleaseGrappleButton";

    [SyncVar] public TipikalPredmet s;

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
        if (!isOwned)
            return;

        if (s == null)
            s = GetComponent<TipikalPredmet>();

        if (s == null || s.usersettingitems == null || s.usersettingitems.player == null)
            return;

        if (s.usersettingitems.player.escaped)
        {
            ReleaseGrapple();
            return;
        }

        if (MobileTaggedInput.WasPressedThisFrame(releaseButtonTag))
            ReleaseGrapple();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public void MobileLeftMouseDownAction()
    {
        if (!isOwned)
            return;

        StartGrapple();
    }

    public void MobileLeftMouseUpAction()
    {
        if (!isOwned)
            return;

        if (s == null || s.itemdat == null || s.usersettingitems == null)
            return;

        s.itemdat.RemoveItems(1);
        if (s.itemdat.amount <= 0)
            s.usersettingitems.ChangeSkin(0);

        ReleaseGrapple();
    }

    public void ReleaseGrapple()
    {
        if (!isOwned)
            return;

        StopGrapple();
    }

    private void StartGrapple()
    {
        if (camera == null)
        {
            if (Camera.main == null)
                return;

            camera = Camera.main.transform;
        }

        if (me == null && s != null)
            me = s.player;
        if (me == null)
            return;

        StopGrappleLocal();

        if (!Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxDistance))
            return;

        isGrappling = true;
        grapplePoint = hit.point;
        sprintJoint = me.AddComponent<SpringJoint>();
        sprintJoint.autoConfigureConnectedAnchor = false;
        sprintJoint.connectedAnchor = grapplePoint;
        sprintJoint.connectedBody = null;
        sprintJoint.maxDistance = 4f;
        sprintJoint.minDistance = 4f;
        sprintJoint.spring = 19f;
        sprintJoint.damper = 15f;
        sprintJoint.massScale = 4.5f;

        if (line != null)
            line.positionCount = 2;

        CMDGrapple(hit.point);
    }

    [Command]
    private void CMDGrapple(Vector3 hit)
    {
        grapplePoint = hit;
        RPCGrapple(hit);
    }

    [ClientRpc]
    private void RPCGrapple(Vector3 hit)
    {
        grapplePoint = hit;
        if (line != null)
            line.positionCount = 2;
    }

    private void OnDisable()
    {
        StopGrappleLocal();
        if (isServer)
            RPCSTOPGrapple();
    }

    private void StopGrapple()
    {
        StopGrappleLocal();
        CMDSTOPGrapple();
    }

    private void StopGrappleLocal()
    {
        if (me != null)
        {
            SpringJoint[] joints = me.GetComponents<SpringJoint>();
            for (int i = 0; i < joints.Length; i++)
            {
                if (joints[i] == null)
                    continue;

                if (joints[i] == sprintJoint || joints.Length == 1)
                    Destroy(joints[i]);
            }
        }

        if (sprintJoint != null)
        {
            sprintJoint.connectedBody = null;
            sprintJoint.connectedAnchor = Vector3.zero;
            Destroy(sprintJoint);
            sprintJoint = null;
        }

        isGrappling = false;
        grapplePoint = Vector3.zero;

        if (line != null)
            line.positionCount = 0;
    }

    [Command]
    private void CMDSTOPGrapple()
    {
        RPCSTOPGrapple();
    }

    [ClientRpc]
    private void RPCSTOPGrapple()
    {
        isGrappling = false;
        grapplePoint = Vector3.zero;
        if (line != null)
            line.positionCount = 0;
    }

    private void DrawRope()
    {
        if (!isGrappling || sprintJoint == null || line == null || gunTip == null)
            return;

        line.SetPosition(0, gunTip.position);
        line.SetPosition(1, grapplePoint);
    }
}
