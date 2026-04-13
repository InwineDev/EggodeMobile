using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class granata : GrenadeType
{
    public float radius = 20f;
    public float force = 500f;

    public float speed = 10f;

    public int damage;
    public GameObject vfx;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
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
        if (vfx != null)
        {
            GameObject vfxx = Instantiate(vfx, gameObject.transform.position, Quaternion.identity);
            NetworkServer.Spawn(vfxx, connectionToClient);
            Destroy(gameObject);
            StartCoroutine(Pon(vfxx));
        } else
        {
            Destroy(gameObject);
        }

    }

    void DAMA3GE(Health sus)
    {
        bool uron = serverProperties.instance.hp;
        if (uron)
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