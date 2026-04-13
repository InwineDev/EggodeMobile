using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DialogeController : MonoBehaviour
{
    private Animator animator;
    public static Action<List<DialogeSettings>> showDialoge;

    [Header("Dialoge Components")]
    public TMP_Text dialogeTextTmp;
    public TMP_Text dialogeAuthorTmp;
    public UnityEngine.UI.Image dialogeImageTmp;
    private AudioSource audioSource;
    public AudioClip[] clips;

    private void OnEnable()
    {
        showDialoge += ShowDialoge;
    }
    private void OnDisable()
    {
        showDialoge -= ShowDialoge;
    }

    void ShowDialoge(List<DialogeSettings> dialogeSettings)
    {
        StopAllCoroutines();
        /*StopCoroutine(ReloadDialoge(dialogeSettings));*/
        StartCoroutine(ReloadDialoge(dialogeSettings));
    }

    IEnumerator ReloadDialoge(List<DialogeSettings> dialogeSettings)
    {
        foreach (var item in dialogeSettings)
        {
            animator.Play("dialogeOpen");
            dialogeTextTmp.text = item.dialogeText;
            dialogeAuthorTmp.text = item.dialogeAuthor;
            dialogeImageTmp.sprite = item.dialogeSprite;
            audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            audioSource.Play();
            float half = item.dialogeTime / 2;
            yield return new WaitForSeconds(half);
            audioSource.Stop();
            yield return new WaitForSeconds(half);
            animator.Play("dialogeClose");
            yield return new WaitForSeconds(0.1f);
        }
        animator.Play("dialogeClose");
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
}
