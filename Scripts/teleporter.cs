using UnityEngine;

public class teleporter : MonoBehaviour
{
    public GameObject targetObject;


    private void OnTriggerEnter(Collider other)
    {
        Vector3 pon = gameObject.transform.position - other.transform.position;
        print(pon);
        other.transform.position = targetObject.transform.position - pon;
    }
}