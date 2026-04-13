using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (!rb) return;
        rb.MovePosition(other.transform.position + new Vector3(0, 5, 0) * Time.deltaTime);
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (!rb) return;
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}
