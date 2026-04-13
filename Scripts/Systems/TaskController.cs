using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TaskController : MonoBehaviour
{
    private Animator animator;
    public static Action<string> showTask;

    [Header("Task Components")]
    public TMP_Text taskText;
    private AudioSource audioSource;

    private void OnEnable()
    {
        showTask += ShowTask;
    }
    private void OnDisable()
    {
        showTask -= ShowTask;
    }

    void ShowTask(string text)
    {
        animator.Play("showTask");
        taskText.text = text;
    }

/*    IEnumerator ReloadDialoge(List<DialogeSettings> dialogeSettings)
    {
        foreach (var item in dialogeSettings)
        {
            animator.Play("dialogeOpen");
            dialogeTextTmp.text = item.dialogeText;
            dialogeAuthorTmp.text = item.dialogeAuthor;
            dialogeImageTmp.sprite = item.dialogeSprite;
            audioSource.Play();
            float half = item.dialogeTime / 2;
            yield return new WaitForSeconds(half);
            audioSource.Stop();
            yield return new WaitForSeconds(half);
            animator.Play("dialogeClose");
            yield return new WaitForSeconds(0.1f);
        }
        animator.Play("dialogeClose");
    }*/
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
}
