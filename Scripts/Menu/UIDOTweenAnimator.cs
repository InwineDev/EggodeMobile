using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

public class UIDOTweenAnimator : MonoBehaviour
{
    public enum AnimationType
    {
        Fade,
        SlideFromLeft,
        SlideFromRight,
        SlideFromTop,
        SlideFromBottom,
        ScaleUp,
        Bounce,
        RotateIn
    }

    [Header("Intro Animation Settings")]
    public AnimationType introAnimationType = AnimationType.Fade;
    public float introDuration = 0.5f;
    public float introDelay = 0f;
    public Ease introEaseType = Ease.OutQuad;

    [Header("Outro Animation Settings")]
    public AnimationType outroAnimationType = AnimationType.Fade;
    public float outroDuration = 0.3f;
    public float outroDelay = 0f;
    public Ease outroEaseType = Ease.InQuad;

    [Header("Slide Settings")]
    public float slideOffset = 100f;

    [Header("Scale Settings")]
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;

    [Header("Bounce Settings")]
    public float bounceStrength = 0.5f;
    public int bounceVibrato = 2;
    public float bounceElasticity = 0.5f;

    [Header("Rotation Settings")]
    public Vector3 startRotation = new Vector3(0, 0, 90f);
    public Vector3 endRotation = Vector3.zero;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 originalRotation;

    [Header("Events")]
    public UnityEvent onIntroComplete;
    public UnityEvent onOutroComplete;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Сохраняем оригинальные значения
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localEulerAngles;
    }

    void OnEnable()
    {
        StartCoroutine(PlayIntroAnimationWithDelay());
    }

    IEnumerator PlayIntroAnimationWithDelay()
    {
        ResetToIntroInitialState();
        yield return new WaitForSeconds(introDelay);
        PlayIntroAnimation();
    }

    public void PlayIntroAnimation()
    {
        switch (introAnimationType)
        {
            case AnimationType.Fade:
                FadeIn();
                break;
            case AnimationType.SlideFromLeft:
                SlideInFromLeft();
                break;
            case AnimationType.SlideFromRight:
                SlideInFromRight();
                break;
            case AnimationType.SlideFromTop:
                SlideInFromTop();
                break;
            case AnimationType.SlideFromBottom:
                SlideInFromBottom();
                break;
            case AnimationType.ScaleUp:
                ScaleIn();
                break;
            case AnimationType.Bounce:
                BounceIn();
                break;
            case AnimationType.RotateIn:
                RotateIn();
                break;
        }
    }

    public void PlayOutroAnimation()
    {
        switch (outroAnimationType)
        {
            case AnimationType.Fade:
                FadeOut(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.SlideFromLeft:
                SlideOutToLeft(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.SlideFromRight:
                SlideOutToRight(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.SlideFromTop:
                SlideOutToTop(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.SlideFromBottom:
                SlideOutToBottom(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.ScaleUp:
                ScaleOut(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.Bounce:
                BounceOut(() => onOutroComplete?.Invoke());
                break;
            case AnimationType.RotateIn:
                RotateOut(() => onOutroComplete?.Invoke());
                break;
        }
    }

    private void ResetToIntroInitialState()
    {
        DOTween.Kill(rectTransform);
        DOTween.Kill(canvasGroup);

        switch (introAnimationType)
        {
            case AnimationType.Fade:
                canvasGroup.alpha = 0;
                break;
            case AnimationType.SlideFromLeft:
                rectTransform.anchoredPosition = originalPosition - new Vector3(slideOffset, 0, 0);
                break;
            case AnimationType.SlideFromRight:
                rectTransform.anchoredPosition = originalPosition + new Vector3(slideOffset, 0, 0);
                break;
            case AnimationType.SlideFromTop:
                rectTransform.anchoredPosition = originalPosition + new Vector3(0, slideOffset, 0);
                break;
            case AnimationType.SlideFromBottom:
                rectTransform.anchoredPosition = originalPosition - new Vector3(0, slideOffset, 0);
                break;
            case AnimationType.ScaleUp:
                rectTransform.localScale = startScale;
                break;
            case AnimationType.Bounce:
                rectTransform.localScale = Vector3.zero;
                break;
            case AnimationType.RotateIn:
                rectTransform.localEulerAngles = startRotation;
                break;
        }
    }

    #region Intro Animations
    private void FadeIn()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void SlideInFromLeft()
    {
        rectTransform.anchoredPosition = originalPosition - new Vector3(slideOffset, 0, 0);
        rectTransform.DOAnchorPos(originalPosition, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void SlideInFromRight()
    {
        rectTransform.anchoredPosition = originalPosition + new Vector3(slideOffset, 0, 0);
        rectTransform.DOAnchorPos(originalPosition, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void SlideInFromTop()
    {
        rectTransform.anchoredPosition = originalPosition + new Vector3(0, slideOffset, 0);
        rectTransform.DOAnchorPos(originalPosition, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void SlideInFromBottom()
    {
        rectTransform.anchoredPosition = originalPosition - new Vector3(0, slideOffset, 0);
        rectTransform.DOAnchorPos(originalPosition, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void ScaleIn()
    {
        rectTransform.localScale = startScale;
        rectTransform.DOScale(endScale, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }

    private void BounceIn()
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(originalScale, introDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(introDelay);
    }

    private void RotateIn()
    {
        rectTransform.localEulerAngles = startRotation;
        rectTransform.DORotate(endRotation, introDuration).SetEase(introEaseType).SetDelay(introDelay);
    }
    #endregion

    #region Outro Animations
    private void FadeOut(System.Action onComplete = null)
    {
        canvasGroup.DOFade(0, outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void SlideOutToLeft(System.Action onComplete = null)
    {
        rectTransform.DOAnchorPos(originalPosition - new Vector3(slideOffset, 0, 0), outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void SlideOutToRight(System.Action onComplete = null)
    {
        rectTransform.DOAnchorPos(originalPosition + new Vector3(slideOffset, 0, 0), outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void SlideOutToTop(System.Action onComplete = null)
    {
        rectTransform.DOAnchorPos(originalPosition + new Vector3(0, slideOffset, 0), outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void SlideOutToBottom(System.Action onComplete = null)
    {
        rectTransform.DOAnchorPos(originalPosition - new Vector3(0, slideOffset, 0), outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void ScaleOut(System.Action onComplete = null)
    {
        rectTransform.DOScale(startScale, outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void BounceOut(System.Action onComplete = null)
    {
        rectTransform.DOScale(0, outroDuration)
            .SetEase(Ease.InBack)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void RotateOut(System.Action onComplete = null)
    {
        rectTransform.DORotate(startRotation, outroDuration)
            .SetEase(outroEaseType)
            .SetDelay(outroDelay)
            .OnComplete(() => onComplete?.Invoke());
    }
    #endregion

    public void ResetToOriginalState()
    {
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        rectTransform.localEulerAngles = originalRotation;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
    }
}