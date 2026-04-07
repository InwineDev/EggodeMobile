
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTagButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string buttonTag;

    private static readonly Dictionary<string, bool> hold = new Dictionary<string, bool>();
    private static readonly Dictionary<string, bool> down = new Dictionary<string, bool>();
    private static readonly Dictionary<string, bool> up = new Dictionary<string, bool>();

    public void OnPointerDown(PointerEventData eventData)
    {
        hold[buttonTag] = true;
        down[buttonTag] = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hold[buttonTag] = false;
        up[buttonTag] = true;
    }

    private void LateUpdate()
    {
        if (down.ContainsKey(buttonTag)) down[buttonTag] = false;
        if (up.ContainsKey(buttonTag)) up[buttonTag] = false;
    }

    public static bool IsPressed(string tag) => hold.TryGetValue(tag, out var value) && value;
    public static bool IsPressedDown(string tag) => down.TryGetValue(tag, out var value) && value;
    public static bool IsReleased(string tag) => up.TryGetValue(tag, out var value) && value;
}
