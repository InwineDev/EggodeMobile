using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class portaldeistvie : MonoBehaviour
{
    public Vector3 sus;
    public bool scene;
    public string scenename;
    private void OnCollisionEnter(Collision collision)
    {
        if (!scene)
        {
            collision.gameObject.transform.position = sus;
        }
        else
        {
            SceneManager.LoadScene(scenename);
        }
    }
}
