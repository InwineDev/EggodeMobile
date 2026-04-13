using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumePlavno : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Volume vol;
    [SerializeField] private GameObject effectButServer;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 0.01f;
    [SerializeField] private float fadeInterval = 0.01f;
    [SerializeField] private float effectDuration = 8f;

    private Coroutine currentFadeCoroutine;

    void OnEnable()
    {
        if (vol == null)
        {
            Debug.LogError("Volume reference is missing!", this);
            return;
        }

        vol.weight = 0;

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeVolume(1f));

        if (isServer)
            CmdEffect();
    }

    [Command]
    private void CmdEffect()
    {
        if (effectButServer != null)
        {
            effectButServer.SetActive(true);
            RpcEffect();
        }
        else
        {
            Debug.LogWarning("effectButServer reference is missing on server!", this);
        }
    }

    [ClientRpc]
    private void RpcEffect()
    {
        if (effectButServer != null)
            effectButServer.SetActive(true);
        else
            Debug.LogWarning("effectButServer reference is missing on client!", this);
    }

    private IEnumerator FadeVolume(float targetWeight)
    {
        while (!Mathf.Approximately(vol.weight, targetWeight))
        {
            vol.weight = Mathf.MoveTowards(vol.weight, targetWeight, fadeSpeed);
            yield return new WaitForSeconds(fadeInterval);
        }

        if (targetWeight > 0.5f)
        {
            yield return new WaitForSeconds(effectDuration);
            currentFadeCoroutine = StartCoroutine(FadeVolume(0f));
        }
    }

    void OnDisable()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
    }
}