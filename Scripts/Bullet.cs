using UnityEngine;
using Mirror;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

public class Bullet : NetworkBehaviour
{
    public int damage = 20;
    public float speed = 10f;
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                bool uron = FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().hp;
                if (uron)
                {
                    print("sus1");
                    health.health -= damage;
                    if (health.health <= 0)
                    {
                        health.health = 100;
                        health.hp.text = $"{health.health} HP";
                    }
                }
                DAMA3GE(health);
            }
        }
        Destroy(gameObject);
    }

    [Command]
    void DAMA3GE(Health sus)
    {
        bool uron = FindFirstObjectByType<serverProperties>().GetComponent<serverProperties>().hp;
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
}