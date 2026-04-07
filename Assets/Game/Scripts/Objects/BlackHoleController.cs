using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : NetworkBehaviour
{
    [SerializeField] private List<Rigidbody> affectedBodies = new List<Rigidbody>();
    private Rigidbody myBody;

    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        affectedBodies.Clear();

        foreach (var item1 in hitColliders)
        {
            if (item1.TryGetComponent(out Rigidbody rigidbody))
            {
                affectedBodies.Add(rigidbody);
            }
        }

        foreach (var rigidbody in affectedBodies)
        {
            Vector3 directionHole = (transform.position - rigidbody.position).normalized;
            float distance = (transform.position - rigidbody.position).magnitude;
            float strenght = rigidbody.mass * myBody.mass / distance;
            rigidbody.AddForce(directionHole * strenght);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.localScale = new Vector3(
            transform.localScale.x + 0.1f,
            transform.localScale.y + 0.1f,
            transform.localScale.z + 0.1f
        );

        if (other.gameObject.tag != "Player")
        {
            NetworkServer.Destroy(other.gameObject);
        }
        else
        {
            DAMA3GE(other.gameObject.GetComponent<Health>());
        }
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.hp)
        {
            print("sus1");
            sus.health -= 1000 + Random.Range(0, 5);
            if (sus.health <= 0)
            {
                sus.health = 100;
                sus.hp.text = $"{sus.health} HP";
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            Vector3.one
        );
        Gizmos.DrawWireSphere(Vector3.zero, 10);
    }
}