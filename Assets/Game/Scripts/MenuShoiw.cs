using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    [Header("Canvas to toggle")]
    public GameObject targetCanvas;

    private bool isOpen = false;

    // Метод для кнопки
    public void Toggle()
    {
        if (targetCanvas == null) return;

        isOpen = !isOpen;
        targetCanvas.SetActive(isOpen);
    }

    // Открыть
    public void Open()
    {
        if (targetCanvas == null) return;

        isOpen = true;
        targetCanvas.SetActive(true);
    }

    // Закрыть
    public void Close()
    {
        if (targetCanvas == null) return;

        isOpen = false;
        targetCanvas.SetActive(false);
    }
}