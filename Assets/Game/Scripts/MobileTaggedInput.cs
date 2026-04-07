using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTaggedInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private static readonly Dictionary<string, MobileTaggedInput> cache = new Dictionary<string, MobileTaggedInput>();

    private string cachedTag;
    private bool held;
    private int downFrame = -1;
    private int upFrame = -1;
    private int activePointerId = int.MinValue;

    public static bool IsHeld(string tag)
    {
        MobileTaggedInput input = Resolve(tag);
        return input != null && input.held;
    }

    public static bool WasPressedThisFrame(string tag)
    {
        MobileTaggedInput input = Resolve(tag);
        return input != null && input.downFrame == Time.frameCount;
    }

    public static bool WasReleasedThisFrame(string tag)
    {
        MobileTaggedInput input = Resolve(tag);
        return input != null && input.upFrame == Time.frameCount;
    }

    public static GameObject FindButtonObject(string tag)
    {
        MobileTaggedInput input = Resolve(tag);
        return input != null ? input.gameObject : null;
    }

    public static MobileTaggedInput Resolve(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return null;

        if (cache.TryGetValue(tag, out MobileTaggedInput existing) && existing != null)
            return existing;

        GameObject found = null;
        try
        {
            found = GameObject.FindGameObjectWithTag(tag);
        }
        catch
        {
            return null;
        }

        if (found == null)
            return null;

        MobileTaggedInput input = found.GetComponent<MobileTaggedInput>();
        if (input == null)
            input = found.AddComponent<MobileTaggedInput>();

        input.cachedTag = tag;
        cache[tag] = input;
        return input;
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        held = false;
        upFrame = Time.frameCount;

        if (!string.IsNullOrEmpty(cachedTag) && cache.TryGetValue(cachedTag, out MobileTaggedInput existing) && existing == this)
            cache.Remove(cachedTag);
    }

    private void Register()
    {
        if (string.IsNullOrEmpty(cachedTag))
            cachedTag = gameObject.tag;

        if (!string.IsNullOrEmpty(cachedTag))
            cache[cachedTag] = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue && activePointerId != eventData.pointerId)
            return;

        activePointerId = eventData.pointerId;
        held = true;
        downFrame = Time.frameCount;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue && activePointerId != eventData.pointerId)
            return;

        held = false;
        upFrame = Time.frameCount;
        activePointerId = int.MinValue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue && activePointerId != eventData.pointerId)
            return;

        held = false;
        upFrame = Time.frameCount;
        activePointerId = int.MinValue;
    }
}
