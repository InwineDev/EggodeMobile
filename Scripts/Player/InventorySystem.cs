using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class inventorySlot
{
    public int amount;
    public GameObject obj;
}

public class InventorySystem : NetworkBehaviour
{
    [SerializeField] private List<inventorySlot> invSlots = new List<inventorySlot>();


}
