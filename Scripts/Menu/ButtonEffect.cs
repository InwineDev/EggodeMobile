using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject toActive;
    public AudioClip audioCLip;
    private AudioSource audioSource;

    [Header("Animation")]
    public float moveDistance = 11f;
    public float duration = 0.25f;
    public Ease easeType = Ease.OutQuad;

    private Vector3 position;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = audioCLip;
        position = transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.Play(0);
        toActive.SetActive(true);
        transform.DOMoveX(position.x + moveDistance, duration)
            .SetEase(easeType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toActive.SetActive(false);
        transform.DOMoveX(position.x, duration)
            .SetEase(easeType);
    }
}