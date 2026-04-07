using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class masloGranata : GrenadeType
{
    public float radius = 20f;
    public float force = 500f;
    public float speed = 10f;
    public GameObject vfx;
    public int damage;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }

    void Explode()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);

        for (int j = 0; j < overlappedColliders.Length; j++)
        {
            Rigidbody rigidbody = overlappedColliders[j].attachedRigidbody;
            if (rigidbody)
            {
                rigidbody.AddExplosionForce(force, transform.position, radius);
                if (rigidbody.GetComponent<userSettingNotCam>())
                {
                    rigidbody.GetComponent<userSettingNotCam>().effectMaslo.SetActive(true);
                }
            }
        }

        if (vfx != null)
        {
            GameObject vfxx = Instantiate(vfx, gameObject.transform.position, Quaternion.identity);
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