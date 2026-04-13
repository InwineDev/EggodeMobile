using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorItemController : MonoBehaviour
{
    [SerializeField] private spawnercube sc;
    private int id;
    [SerializeField] private TMP_Text txtName;
    public void init(int localID, spawnercube s)
    {
        id = localID;
        sc = s;
        txtName.text = sc.cubessssss[localID].name;
    }

    public void ChangeOBJ()
    {
        sc.sugomamami(id);
    }
}
