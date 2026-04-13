using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KtController : NetworkBehaviour
{
    private Image image;
    private Sequence currentAnimation;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void PlayCenterAnimation(float animationDuration)
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill(true);
        }
        transform.localScale = Vector3.zero;
        image.fillAmount = 1;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

        // Создаем новую последовательность анимаций
        currentAnimation = DOTween.Sequence();

        // 1. Увеличение из центра
        currentAnimation.Append(
            transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), animationDuration / 4)
                .SetEase(Ease.OutBack)
                .OnKill(() => transform.localScale = new Vector3(0.25f, 0.25f, 0.25f)));

        // 2. Уменьшение как торт (равномерное сжатие)
        currentAnimation.Append(
            image.DOFillAmount(0f, (animationDuration / 4) * 3)
                .SetEase(Ease.InOutQuad))
                .OnKill(() => image.fillAmount = 0f);

        // 3. Уход обратно в центр
        currentAnimation.Append(
            transform.DOScale(Vector3.zero, 1)
                .SetEase(Ease.InBack))
                .OnKill(() => transform.localScale = Vector3.zero);

        // Очищаем ссылку при завершении анимации
        currentAnimation.OnComplete(() => currentAnimation = null);
    }
}
