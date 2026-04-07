using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class PistolController : NetworkBehaviour
{
    public AudioSource ad;
    [SerializeField] private float kt = 1f;
    public bool ktbool;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private int damage = 20;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject effectToSpawn;

    [SyncVar]
    public TipikalPredmet s;

    [Command]
    void CmdSpawn()
    {
        RpcSpawn();
    }

    [ClientRpc]
    void RpcSpawn()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (ad != null && clips.Count > 0)
            ad.clip = clips[Random.Range(0, clips.Count)];
        if (effect != null)
            effect.SetActive(true);
        StartCoroutine(meme());
        if (ad != null)
            ad.Play(0);

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (effectToSpawn)
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                GameObject sled = Instantiate(effectToSpawn, hit.point, rotation);
                NetworkServer.Spawn(sled);
                sled.transform.parent = hit.transform;
            }

            if (hit.transform.gameObject.GetComponent<Health>() != null)
                DAMAGE(hit.transform.gameObject.GetComponent<Health>(), damage);

            if (hit.transform.gameObject.GetComponent<name24>() != null)
                DAMAGEITEM(hit.transform.gameObject.GetComponent<name24>());
        }
    }

    [Command]
    void DAMAGE(Health health, int damage1)
    {
        health.DAMA3GE(damage1);
    }

    [Command]
    void DAMAGEITEM(name24 sus)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.destroy)
        {
            print("sus1");
            sus._hp -= damage + Random.Range(0, 5);
        }
    }

    void Update()
    {
        if (s == null) s = GetComponent<TipikalPredmet>();
        if (s == null || s.usersettingitems == null || s.usersettingitems.player == null) return;
        if (s.usersettingitems.player.escaped) return;
    }

    public void MobileLeftMouseDownAction()
    {
        if (!isOwned) return;
        if (s == null || s.itemdat == null || s.usersettingitems == null) return;
        if (ktbool) return;

        CmdSpawn();
        s.itemdat.RemoveItems(1);
        if (s.itemdat.amount <= 0)
            s.usersettingitems.ChangeSkin(0);

        StartCoroutine(kttime());
        ktbool = true;
    }

    private IEnumerator kttime()
    {
        s.usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
        if (effect != null)
            effect.SetActive(false);
    }

    private IEnumerator meme()
    {
        yield return new WaitForSeconds(0.1f);
        if (effect != null)
            effect.SetActive(false);
    }

    private void OnEnable()
    {
        ChangeEffect();
        if (ktbool)
            StartCoroutine(kttime());
    }

    [ClientRpc]
    void ChangeEffect()
    {
        StartCoroutine(meme());
    }
}
