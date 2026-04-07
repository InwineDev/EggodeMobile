using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnerBomba : NetworkBehaviour
{
    //[SyncVar(hook = nameof(OnMamaChanged))]
    public GameObject mama;

    public GameObject cam;

    [SerializeField] private float kt = 3f;
    public bool ktbool;

    [SyncVar]
    public TipikalPredmet s;

    // ��� ��� ������������� mama
    private void OnMamaChanged(GameObject oldValue, GameObject newValue)
    {
        mama = newValue;
        // ����� �������� �������������� ������ ��� ���������
    }

    [Command]
    void CmdSetMama(GameObject newMama)
    {
        if(newMama) mama = newMama;
    }

    [Command]
    void CmdSpawn()
    {
        if(mama == null) return;
        Vector3 ma = gameObject.transform.position + gameObject.transform.forward * 2f;
        GameObject koshka = Instantiate(mama, ma, cam.transform.rotation);

        if (koshka.TryGetComponent(out GrenadeType grenade))
        {
            grenade.player = GetComponent<TipikalPredmet>().player;
        }

        EventBus.OnObjectSpawn?.Invoke(koshka);
        NetworkServer.Spawn(koshka);
        RpcSPAWNITEMS();
    }

    void Update()
    {
        if (!isOwned)
            return;
    }


    public void MobileRightMouseDownAction()
    {
        if (!isOwned) return;
        if (ktbool) return;

        if (mama == null)
        {
            Debug.LogError("Cannot spawn - mama is null!");
            return;
        }

        CmdSpawn();
/*            if (s != null && s.itemdat != null)
        {
            s.itemdat.RemoveItems(1);

            if (s.itemdat.amount <= 0 && s.usersettingitems != null)
            {
                s.usersettingitems.ChangeSkin(0);
            }
        }*/

        StartCoroutine(kttime());
        ktbool = true;
    }

    private IEnumerator kttime()
    {
        s.usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
    }

    private void OnEnable()
    {
        cam = s.usersettingitems.cam.gameObject;

        s = GetComponent<TipikalPredmet>();
        if (s == null)
        {
            Debug.LogError("TipikalPredmet component not found!");
            return;
        }

        // ��������� mama �� �������
        if (isServer)
        {
            if (s.spawn)
                mama = s.spawn;
        }
        else
        {
            CmdSetMama(s.spawn);
        }

        // ����������� �������
        var networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found!");
            return;
        }

        if (s.spawn != null && !networkManager.spawnPrefabs.Contains(s.spawn))
        {
            networkManager.spawnPrefabs.Add(s.spawn);
        }

        Debug.Log($"Mama set to: {s.spawn}");

        if (ktbool)
        {
            StartCoroutine(kttime());
        }
    }

    [TargetRpc]
    void RpcSPAWNITEMS()
    {
        if (s != null && s.itemdat != null)
        {
            s.itemdat.RemoveItems(1);

            if (s.itemdat.amount <= 0 && s.usersettingitems != null)
            {
                s.usersettingitems.ChangeSkin(0);
            }
        }
    }
}