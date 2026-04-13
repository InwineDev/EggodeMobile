using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class physgun : NetworkBehaviour
{
    public Camera cam;
    public float mgd = 10f, tf = 20f, ls = 50f;
    public Transform oH;

    public Rigidbody gRB;

    private void Start()
    {
        cam = Camera.main;
    }

    //[SerializeField] private LineRenderer lineEnd;
    void Update()
    {
        if (isOwned)
        {
            if (gRB)
            {
                CmdMoveObj(gRB.gameObject, oH.transform.position, oH.transform.rotation);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                oH.transform.rotation = new Quaternion(oH.transform.rotation.w, oH.transform.rotation.x + 90, oH.transform.rotation.y, oH.transform.rotation.z);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (gRB)
                {  
                    CmdRemoveObj(gRB.gameObject);
                    gRB = null;
                }
                else
                {
                        RaycastHit hit;
                        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                        if (Physics.Raycast(ray, out hit, mgd))
                        {
                            if (hit.collider.gameObject.GetComponent<Rigidbody>())
                            {
                                gRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                                if (gRB)
                                {
                                    CmdGiveObj(gRB.gameObject);
                                }
                            }
                        }
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (gRB)
                {
                    gRB = null;
                    //lineEnd.gameObject.SetActive(false);
                }
            }
            
        }
    }
    [Command]
    void CmdRemoveObj(GameObject gRB2)
    {
        gRB2.GetComponent<Rigidbody>().isKinematic = false;
        RPCRemoveObj(gRB2);
    }

    [Command]
    void CmdGiveObj(GameObject gRB2)
    {
        if (gRB2 == null || gRB2.GetComponent<NetworkIdentity>() == null)
        {
            Debug.LogWarning("Invalid GameObject provided to CmdGiveObj");
            return;
        }
        if (gRB2.GetComponent<Rigidbody>() != null)
        {
            //oH.transform.localRotation = gRB2.transform.rotation;
            gRB2.GetComponent<Rigidbody>().isKinematic = true;
            RPCGiveObj(gRB2);
        }
        else
        {
            Debug.Log("Error: CmdGiveObj - GameObject is null or has no Rigidbody.");
        }
    }

    [Command]
    void CmdMoveObj(GameObject gRB2, Vector3 oH2, Quaternion oH3)
    {
        if (gRB2.GetComponent<Rigidbody>() != null)
        {
            gRB2.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(gRB2.transform.position, oH2, Time.deltaTime * ls));
            gRB2.GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(gRB2.transform.rotation, oH3, Time.deltaTime * ls));
            RPCMoveObj(gRB2, oH2, oH3);
        }
        else
        {
            Debug.Log("Error: CmdMoveObj - GameObject is null or has no Rigidbody.");
        }
    }

    [ClientRpc]
    void RPCRemoveObj(GameObject gRB2)
    {
        gRB2.GetComponent<Rigidbody>().isKinematic = false;
       // lineEnd.gameObject.SetActive(false);
    }

    [ClientRpc]
    void RPCGiveObj(GameObject gRB2)
    {
        //oH.transform.localRotation = gRB2.transform.localRotation;
        gRB2.GetComponent<Rigidbody>().isKinematic = true;
        //lineEnd.gameObject.SetActive(true);
    }

    [ClientRpc]
    void RPCMoveObj(GameObject gRB2, Vector3 oH2, Quaternion oH3)
    {
       // lineEnd.SetPosition(1, oH.transform.localPosition);
        gRB2.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(gRB2.transform.position, oH2, Time.deltaTime * ls));
        gRB2.GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(gRB2.transform.rotation, oH3, Time.deltaTime * ls));
    }
}
