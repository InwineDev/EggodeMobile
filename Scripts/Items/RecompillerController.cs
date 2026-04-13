using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecompillerController : NetworkBehaviour, interactable
{
    [SerializeField] private List<RecompillerElement> elements = new List<RecompillerElement>();
    [SerializeField] private Transform firstPosition;
    [SerializeField] private Transform secondPosition;
    [SerializeField] private Transform exitPosition;
    public void interact(FirstPersonController player)
    {
        try
        {
            GameObject spawn = Instantiate(SpawnObject(), exitPosition.position, exitPosition.rotation);
            NetworkServer.Spawn(spawn);
        }
        catch
        {

        }
    }

    private GameObject SpawnObject()
    {
        try
        {
            string name0 = "";
            string name1 = "";

            GameObject zero = null;
            GameObject one = null;

            Collider[] hitColliders = Physics.OverlapBox(firstPosition.position, firstPosition.localScale, firstPosition.rotation);
            foreach (var item1 in hitColliders)
            {
                if (item1.GetComponent<name24>() & item1.gameObject != gameObject)
                {
                    name0 = item1.GetComponent<name24>().name244;
                    zero = item1.gameObject;
                }
            }

            Collider[] hitColliders1 = Physics.OverlapBox(secondPosition.position, secondPosition.localScale, secondPosition.rotation);
            foreach (var item1 in hitColliders1)
            {
                if (item1.GetComponent<name24>() & item1.gameObject != gameObject)
                {
                    name1 = item1.GetComponent<name24>().name244;
                    one = item1.gameObject;
                }
            }

            print(name0 + " + " + name1);

            foreach (var item1 in elements)
            {
                if (item1.puzzles.Contains(name0))
                {
                    if (item1.puzzles.Contains(name1))
                    {
                        NetworkServer.Destroy(zero);
                        NetworkServer.Destroy(one);
                        return item1.exitObject;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            print(e);
            return null;
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(
            firstPosition.position,
            firstPosition.rotation,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, firstPosition.localScale);

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(
            secondPosition.position,
            secondPosition.rotation,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, secondPosition.localScale);
    }
}
