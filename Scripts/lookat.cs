using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class lookat : MonoBehaviour
{
    public GameObject target;

    private void Start()
    {
        StartCoroutine(Timetohahaha());
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        Vector3 direction = target.transform.position - transform.position;

        direction.Normalize();

        transform.Translate(direction * speed * Time.deltaTime);
    }
    float maxDistance = 5;


    void FixedUpdate()
    {

        RaycastHit hit;
        print("hitted!");
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance) &&
                    hit.collider.gameObject.CompareTag("Player"))
        {

            string error = "Невозможно присвоить объект Факел игроку Факельник";
            print("hitted player!");
            hit.collider.gameObject.GetComponent<userSettingNotCam>().Critical(error);
            StartCoroutine(Deactivate());

        }

    }
    IEnumerator Timetohahaha()
    {
        yield return new WaitForSeconds(5);
        speed = 50f;
    }
    public AudioSource sus;

    IEnumerator Deactivate()
    {

        sus.Play();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);

    }

    public float speed = 5.0f;
}
