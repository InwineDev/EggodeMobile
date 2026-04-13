using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image hpImage;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float delayBeforeRecovery = 2f; // Задержка перед восстановлением (2 секунды)
    [SerializeField] private float recoveryDuration = 1f; // Длительность анимации восстановления

    private Sequence currentAnimation;
    private float targetFillAmount;
    private Tween recoveryTween;

    private int maxHp = 100;

    private void OnEnable()
    {
        health.OnDamage += StartAnimation;
        UpdateHPDisplayImmediately();
    }

    private void OnDisable()
    {
        health.OnDamage -= StartAnimation;
        currentAnimation?.Kill();
        recoveryTween?.Kill();
    }

    private void StartAnimation(int oldHp, int newHp)
    {
        // Прерываем текущие анимации
        currentAnimation?.Kill();
        recoveryTween?.Kill();

        // Создаем новую последовательность анимаций
        currentAnimation = DOTween.Sequence();

        // Анимация уменьшения HP (если получили урон)
        if (newHp < oldHp)
        {
            targetFillAmount = (float)newHp / maxHp;
            // Сначала быстро уменьшаем полоску
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount, animationDuration * 0.7f)
                    .SetEase(Ease.OutQuad));

/*            // Затем небольшой "отскок" (эффект упругости)
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount * 1.05f, animationDuration * 0.15f)
                    .SetEase(Ease.OutQuad));*/

            // Возвращаем к точному значению
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount, animationDuration * 0.15f)
                    .SetEase(Ease.InQuad));

            // Запускаем восстановление через delayBeforeRecovery секунд
            recoveryTween = DOVirtual.DelayedCall(delayBeforeRecovery, () =>
            {
                RecoveryAnimation();
            });
        }
        // Анимация увеличения HP (если получили лечение)
        else if (newHp > oldHp)
        {
            targetFillAmount = (float)oldHp / newHp;
            // Сначала быстро уменьшаем полоску
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount, animationDuration * 0.7f)
                    .SetEase(Ease.OutQuad));

/*            // Затем небольшой "отскок" (эффект упругости)
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount * 1.05f, animationDuration * 0.15f)
                    .SetEase(Ease.OutQuad));*/

            // Возвращаем к точному значению
            currentAnimation.Append(
                hpImage.DOFillAmount(targetFillAmount, animationDuration * 0.15f)
                    .SetEase(Ease.InQuad));

            // Запускаем восстановление через delayBeforeRecovery секунд
            recoveryTween = DOVirtual.DelayedCall(delayBeforeRecovery, () =>
            {
                RecoveryAnimation();
            });
        }

        currentAnimation.OnComplete(() => currentAnimation = null);
        maxHp = newHp;
    }

    private void RecoveryAnimation()
    {
        // Анимация плавного восстановления до полного HP
        currentAnimation = DOTween.Sequence()
            .Append(hpImage.DOFillAmount(1f, recoveryDuration).SetEase(Ease.OutQuad))
            .OnComplete(() => {
                currentAnimation = null;
                health.health = maxHp; // Обновляем значение здоровья
            });
    }

    private void UpdateHPDisplayImmediately()
    {
        if (health != null && hpImage != null)
        {
            hpImage.fillAmount = (float)health.health / maxHp;
        }
    }
}