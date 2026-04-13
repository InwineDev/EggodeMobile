using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class item
{
    public int id;
    public int amount;
}

public class ChestController : NetworkBehaviour
{
    public readonly SyncList<item> items = new SyncList<item>();

    [Server]
    public void AddItem(int id, userSettings s)
    {
        if (!isServer || s == null) return;

        // Изменяем через временную переменную
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                var temp = items[i];
                temp.amount++;
                items[i] = temp;
                return;
            }
        }
        items.Add(new item { id = id, amount = 1 });
    }

    [Server]
    public int GetItems(int id, userSettings s)
    {
        if (!isServer || s == null) return 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                var temp = items[i];
                temp.amount--;
                int newAmount = temp.amount;

                if (temp.amount <= 0)
                    items.RemoveAt(i);
                else
                    items[i] = temp;

                return newAmount;
            }
        }
        return 0;
    }
}