using UnityEngine;
using Mirror;

public class HideObjectForOthers : NetworkBehaviour
{
    private void Start()
    {
        // Если объект не принадлежит локальному игроку, отключаем его
        if (!isLocalPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}