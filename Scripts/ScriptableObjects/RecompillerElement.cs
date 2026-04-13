using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RecompillerElement", menuName = "Eggode 2/New RecompillerElement")]
public class RecompillerElement : ScriptableObject
{
    public GameObject exitObject;
    public int howMany = 1;
    public List<string> puzzles = new List<string>(2);
    private void OnValidate()
    {
        if (puzzles.Count >= 3)
        {
            puzzles.RemoveAt(0);
        }
    }
}
