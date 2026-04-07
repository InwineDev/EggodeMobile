using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class ShotgunController : NetworkBehaviour
{
    public AudioSource ad;
    [SerializeField] private float kt = 1f;
    public bool ktbool;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private int damage = 15;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject effectToSpawn;
    [SerializeField] private Animator animator;
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 10f;

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
        if (ad != null && clips.Count > 0)
            ad.clip = clips[Random.Range(0, clips.Count)];
        if (effect) effect.SetActive(true);
        if (animator != null) animator.Play("shotgunShoot");
        StartCoroutine(meme());
        if (ad != null) ad.Play(0);

        if (s == null || s.usersettingitems == null || s.usersettingitems.cam == null) return;

        for (int i = 0; i < pelletCount; i++)
        {
            Camera cam = s.usersettingitems.cam;
            Vector3 direction = cam.transform.forward;

            direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), cam.transform.up) * direction;
            direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), cam.transform.right) * direction;

            Ray ray = new Ray(cam.transform.position, direction);
            RaycastHit hit;

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
                    DAMA3GE(hit.transform.gameObject.GetComponent<Health>());

                if (hit.transform.gameObject.GetComponent<name24>() != null)
                    DAMAGEITEM(hit.transform.gameObject.GetComponent<name24>());
            }
        }
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

    [Command]
    void DAMA3GE(Health sus)
    {
        serverProperties props = FindObjectOfType<serverProperties>();
        if (props != null && props.hp)
        {
            print("sus1");
            sus.health -= damage + Random.Range(0, 5);
            if (sus.health <= 0)
            {
                sus.health = 100;
                sus.hp.text = $"{sus.health} HP";
            }
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
        if (s == null || s.itemdat == null || s.usersettingitems == null || s.usersettingitems.player == null) return;
        if (ktbool) return;

        if (Camera.main != null)
        {
            Vector3 recoilDirection = -Camera.main.transform.forward;
            s.usersettingitems.player.rb.AddForce(recoilDirection * 5f, ForceMode.Impulse);
        }

        CmdSpawn();
        s.itemdat.RemoveItems(1);

        if (s.itemdat.amount <= 0)
            s.usersettingitems.ChangeSkin(0);

        StartCoroutine(kttime());
        ktbool = true;
    }

    private IEnumerator kttime()
    {
        if (animator != null) animator.Play("shotgunReload");
        s.usersettingitems.OnKtStart?.Invoke(kt);
        yield return new WaitForSeconds(kt);
        ktbool = false;
        if (effect) effect.SetActive(false);
    }

    private IEnumerator meme()
    {
        yield return new WaitForSeconds(0.1f);
        if (effect) effect.SetActive(false);
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
