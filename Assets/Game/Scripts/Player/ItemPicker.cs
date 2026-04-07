using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : NetworkBehaviour
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

            if (Input.GetKeyDown(KeyCode.F))
            {
                MobileFAction();
            }
        }
    }

    public void MobileFAction()
    {
        if (!isOwned)
            return;

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
                if (hit.collider.gameObject.GetComponent<Rigidbody>() & hit.collider.gameObject.tag == "object")
                {
                    gRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                    if (gRB)
                    {
                        gRB.GetComponent<Collider>().isTrigger = true;
                        CmdGiveObj(gRB.gameObject);
                    }
                }
            }
        }
    }

    [Command]
    void CmdRemoveObj(GameObject gRB2)
    {
        //gRB2.GetComponent<Rigidbody>().isKinematic = false;
        //gRB2.GetComponent<Collider>().isTrigger = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = GetRaycastHitPoint(ray, mgd, gRB2.transform);

        // Çàïóñêàåì êîðóòèíó íà êëèåíòå ÷åðåç RPC
        RPCMoveObjSmoothly(gRB2, targetPosition);
    }

    [ClientRpc]
    void RPCMoveObjSmoothly(GameObject obj, Vector3 targetPos)
    {
        StartCoroutine(MoveObjectSmoothly(obj, targetPos, 0.25f));
    }

    IEnumerator MoveObjectSmoothly(GameObject obj, Vector3 targetPos, float duration)
    {
        Vector3 startPos = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ôèíàëèçèðóåì ïîçèöèþ
        obj.transform.position = targetPos;
        yield return new WaitForSeconds(0.1f);
        // Âûçûâàåì îðèãèíàëüíûé RPC
        RPCRemoveObj(obj);
    }

    Vector3 GetRaycastHitPoint(Ray ray, float distance, Transform obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            return hit.point + hit.normal * obj.position.magnitude / 11;
        }
        else
        {
            return ray.origin + ray.direction * distance;
        }
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
            gRB2.GetComponent<Collider>().isTrigger = true;
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
        gRB2.GetComponent<Collider>().isTrigger = false;
        gRB2.tag = "object";
        // lineEnd.gameObject.SetActive(false);
    }

    [ClientRpc]
    void RPCGiveObj(GameObject gRB2)
    {
        //oH.transform.localRotation = gRB2.transform.localRotation;
        gRB2.GetComponent<Rigidbody>().isKinematic = true;
        gRB2.GetComponent<Collider>().isTrigger = true;
        //lineEnd.gameObject.SetActive(true);
        gRB2.tag = "Untagged";
    }

    [ClientRpc]
    void RPCMoveObj(GameObject gRB2, Vector3 oH2, Quaternion oH3)
    {
        // lineEnd.SetPosition(1, oH.transform.localPosition);
        gRB2.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(gRB2.transform.position, oH2, Time.deltaTime * ls));
        gRB2.GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(gRB2.transform.rotation, oH3, Time.deltaTime * ls));
    }
}
