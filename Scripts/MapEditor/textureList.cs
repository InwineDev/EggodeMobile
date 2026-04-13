using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textureList : MonoBehaviour
{
    [SerializeField] private GameObject texturePrefab;

    public List<textureController> textures = new List<textureController>();

    [SerializeField] private GameObject content;

    public void createTexture()
    {
        GameObject newTexture = Instantiate(texturePrefab);
        newTexture.transform.parent = content.transform;
        textures.Add(newTexture.GetComponent<textureController>());
    }

    public void DestroyTexture(textureController toDestroy)
    {
        textures.Remove(toDestroy);
        Destroy(toDestroy.gameObject);
    }

    public void loadTexture(string namee, byte[] bytes, int wi, int hei)
    {
        GameObject newTexture = Instantiate(texturePrefab);
        newTexture.transform.parent = content.transform;
        newTexture.GetComponent<textureController>().myname = namee;
        newTexture.GetComponent<textureController>().textureBytes = bytes;
        newTexture.GetComponent<textureController>().width = wi;
        newTexture.GetComponent<textureController>().height = hei;
        newTexture.GetComponent<textureController>().reloadInfo();
        textures.Add(newTexture.GetComponent<textureController>());
    }
}
