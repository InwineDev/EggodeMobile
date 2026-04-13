using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : NetworkBehaviour
{

    public float kt = 10f;
    public bool ktbool;
    public Animator animka;

    public AudioSource nuh;

    [SerializeField] private TipikalPredmet s;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isOwned)
            {
                if (ktbool == false)
                {
                    animka.Play("drink");
                    ktbool = true;
                    GIVEHP();
                    StartCoroutine(kttime());
                }
            }

        }
    }

    [Command]
    void GIVEHP()
    {
        nuh.Play(0);
        GIVERPC();
    }

    [ClientRpc]
    void GIVERPC()
    {
        nuh.Play(0);
    }

    private IEnumerator kttime()
    {
        s.usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
    }

    private void OnEnable()
    {
        animka = GetComponent<TipikalPredmet>().animka;
        if (ktbool)
        {
            StartCoroutine(kttime());
        }
    }
}
