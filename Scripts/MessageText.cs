using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageText : NetworkBehaviour
{
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = messageText.transform.localPosition;
    }
    public void ShowMessage(string text, float time)
    {
        RpcMessage(text, time);
    }

    [ClientRpc]
    void RpcMessage(string text, float time)
    {
        messageText.text = text;
        // Прерываем предыдущие анимации
        messageText.DOKill();
        messageText.transform.DOKill();

        // Устанавливаем начальное положение (ниже оригинального)
        messageText.transform.localPosition = originalPosition - new Vector3(0, moveDistance, 0);

        // Последовательность анимаций
        Sequence sequence = DOTween.Sequence();

        // Анимация появления и движения
        sequence.Append(messageText.DOFade(1f, fadeDuration));
        sequence.Join(messageText.transform.DOLocalMoveY(originalPosition.y, fadeDuration).SetEase(easeType));

        // Задержка перед исчезновением
        sequence.AppendInterval(time);

        // Анимация исчезновения
        sequence.Append(messageText.DOFade(0f, fadeDuration));
        sequence.Join(messageText.transform.DOLocalMoveY(originalPosition.y + moveDistance / 2f, fadeDuration));

        // Возвращаем в исходное состояние по завершении
        sequence.OnComplete(() => {
            messageText.text = "";
        });
    }
}
