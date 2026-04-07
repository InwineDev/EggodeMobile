using Mirror;
using Mirror.BouncyCastle.Asn1.Pkcs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : NetworkBehaviour
{
    public float radius = 20f;
    public float force = 500f;

    public int damage;
    public GameObject effect;

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Explode()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);

        for (int j = 0; j < overlappedColliders.Length; j++)
        {
            Rigidbody rigidbody = overlappedColliders[j].attachedRigidbody;
            if (rigidbody)
            {
                rigidbody.AddExplosionForce(force, transform.position, radius);
                if (rigidbody.GetComponent<Health>())
                {
                    DAMA3GE(rigidbody.GetComponent<Health>());
                }
            }
        }

        if (effect != null)
        {
            GameObject vfxx = Instantiate(effect, gameObject.transform.position, Quaternion.identity);
            NetworkServer.Spawn(vfxx, connectionToClient);
            Destroy(gameObject);
            StartCoroutine(Pon(vfxx));
        }
    }

    void DAMA3GE(Health sus)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.hp)
        {
            print("sus1");
            sus.health -= damage;
            if (sus.health <= 0)
            {
                sus.health = 100;
                sus.hp.text = $"{sus.health} HP";
            }
        }
    }

    IEnumerator Pon(GameObject vfxx)
    {
        yield return new WaitForSeconds(5);
        NetworkServer.Destroy(vfxx);
    }
}