using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drinkbanka : NetworkBehaviour
{

    [SerializeField] private float kt = 10f;
    public bool ktbool;
    public Animator animka;
    public int hill = 10;


    public AudioSource drinchik;

    [SerializeField] private TipikalPredmet s;


    private void Start()
    {
        s = GetComponent<TipikalPredmet>();
    }

    void Update()
    {
        float animationTime = s.animka.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animationTime >= 1f)
        {
            if (s.itemdat.amount <= 0)
            {
                s.usersettingitems.ChangeSkin(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            MobileFAction();
        }
    }

    public void MobileFAction()
    {
        if (!isOwned)
            return;

        if (ktbool)
            return;

        if (s == null)
            s = GetComponent<TipikalPredmet>();

        if (s == null)
            return;

        if (s.animka)
            s.animka.Play("drink");

        ktbool = true;
        StartCoroutine(kttime());
        GIVEHP(gameObject.GetComponent<TipikalPredmet>().player.GetComponent<Health>());
        s.itemdat.RemoveItems(1);
    }


    [Command]
    void GIVEHP(Health sus)
    {
        print("sus1");
        sus.health += hill;
        GIVERPC();
    }

    [ClientRpc]
    void GIVERPC()
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
        if (ktbool)
        {
            StartCoroutine(kttime());
        }
    }

}
