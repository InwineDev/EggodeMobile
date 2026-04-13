using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class masloController : NetworkBehaviour
{

    public float kt = 10f;
    public bool ktbool;
    public Animator animka;

    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject effectButServer;

    public AudioSource drinchik;

    [SerializeField] private TipikalPredmet s;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isOwned)
            {
                if (ktbool == false)
                {
                    if(s.animka) s.animka.Play("drink");
                    ktbool = true;
                    Invoke("ShowEffect", 0.5f);
                    CmdEffect();
                    StartCoroutine(kttime());
                    s.itemdat.RemoveItems(1);
                    if (s.itemdat.amount <= 0)
                    {
                        s.usersettingitems.ChangeSkin(0);
                    }
                }
            }

        }
    }

    void ShowEffect()
    {
        effect.SetActive(true);
    }

    [Command]
    void CmdEffect()
    {
        RpcEffect();
    }

    [ClientRpc]
    void RpcEffect()
    {
        drinchik.Play(0);
    }

    private IEnumerator kttime()
    {
        s.usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
    }

    private void OnEnable()
    {
        effect = s.player.GetComponent<userSettingNotCam>().effectMaslo;
        if (ktbool)
        {
            StartCoroutine(kttime());
        }
    }

}
