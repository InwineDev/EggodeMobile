using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Eggode 2/New LootTable")]
public class LootTable : ScriptableObject
{
    public List<lootClass> loot = new List<lootClass>();
    public int maxLoot;
    public int minLoot = 0;
}

[System.Serializable]
public class lootClass
{
    public GameObject lootPrefab;
    public float chance;
}