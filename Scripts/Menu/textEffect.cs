using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class textEffect : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private RectTransform firstPos;
    [SerializeField] private RectTransform secondPos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Invoke("ChangePosition", 0.2f);
    }

    void ChangePosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float randomX = Random.Range(firstPos.anchoredPosition.x, secondPos.anchoredPosition.x);
        float randomY = Random.Range(firstPos.anchoredPosition.y, secondPos.anchoredPosition.y);

        rectTransform.anchoredPosition = new Vector2(randomX, randomY);
    }
}