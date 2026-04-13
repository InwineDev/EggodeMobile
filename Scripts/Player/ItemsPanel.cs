using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsPanel : MonoBehaviour
{
    [SerializeField] private userSettings us;
    [SerializeField] private Image panelImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text hotkeysText;
    [SerializeField] private TMP_Text howManyItems;
    [SerializeField] private TMP_Text itemName;
    private Sequence _currentFadeSequence;

    private itemdannie selectedObject;

    private void Start()
    {
        Invoke("Starter", 0.5f);
    }
    private void Starter()
    {
        ChangeItem(0);
    }
    private void OnEnable()
    {
        if (us)
            us.OnChangeItem += ChangeItem;
    }

    private void OnDisable()
    {
        if (us)
            us.OnChangeItem -= ChangeItem;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) & Input.GetKey(KeyCode.F8))
        {
            gameObject.SetActive(false);
        }
    }
    private void ChangeItem(int id)
    {
    // Отменяем предыдущую анимацию, если она была
    if (_currentFadeSequence != null && _currentFadeSequence.IsActive())
    {
        _currentFadeSequence.Kill();
    }

    // Отписываемся от предыдущего события
    if (selectedObject != null)
    {
        selectedObject.ChangeAmount -= ChangeAmount;
    }

    // Устанавливаем новые значения
    selectedObject = us.idannie[id].itemdat;
    panelImage.sprite = us.idannie[id].texture;
    howManyItems.text = selectedObject.amount.ToString();
    hotkeysText.text = us.idannie[id].helpText;
    itemName.text = us.idannie[id].itemName;
    selectedObject.ChangeAmount += ChangeAmount;

    // Сбрасываем прозрачность перед новой анимацией
    backgroundImage.color = new Color(1f, 1f, 1f, 1);
    panelImage.color = new Color(1f, 1f, 1f, 1);
    hotkeysText.color = new Color(1f, 1f, 1f, 1);
    howManyItems.color = new Color(1f, 1f, 1f, 1);
        itemName.color = new Color(1f, 1f, 1f, 1);

    // Создаем новую последовательность анимаций
    _currentFadeSequence = DOTween.Sequence()
        .Append(backgroundImage.DOColor(new Color(1f, 1f, 1f, 0.2f), 3f).SetEase(Ease.InQuad))
        .Join(panelImage.DOColor(new Color(1f, 1f, 1f, 0.2f), 3f).SetEase(Ease.InQuad))
        .Join(hotkeysText.DOColor(new Color(1f, 1f, 1f, 0.2f), 3f).SetEase(Ease.InQuad))
        .Join(itemName.DOColor(new Color(1f, 1f, 1f, 0.2f), 3f).SetEase(Ease.InQuad))
        .Join(howManyItems.DOColor(new Color(1f, 1f, 1f, 0.2f), 3f).SetEase(Ease.InQuad))
        .OnComplete(() => _currentFadeSequence = null);

    // Добавляем небольшую задержку перед началом исчезновения
    _currentFadeSequence.PrependInterval(1f);
    }

    private void ChangeAmount(int howMany)
    {
        howManyItems.text = selectedObject.amount.ToString();
    }

}
