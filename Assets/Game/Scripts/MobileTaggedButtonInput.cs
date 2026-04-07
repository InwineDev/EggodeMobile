using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MobileTaggedButtonInput
{
    private class ButtonState
    {
        public bool held;
        public int downFrame = -1;
        public int upFrame = -1;
        public MobileTaggedButtonRelay relay;
    }

    private static readonly Dictionary<string, ButtonState> states = new Dictionary<string, ButtonState>();

    private static ButtonState GetState(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return null;

        if (!states.TryGetValue(tag, out ButtonState state))
        {
            state = new ButtonState();
            states[tag] = state;
        }

        return state;
    }

    public static void EnsureRelay(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return;

        ButtonState state = GetState(tag);
        if (state == null)
            return;

        if (state.relay != null)
            return;

        GameObject obj;
        try
        {
            obj = GameObject.FindGameObjectWithTag(tag);
        }
        catch
        {
            return;
        }

        if (obj == null)
            return;

        MobileTaggedButtonRelay relay = obj.GetComponent<MobileTaggedButtonRelay>();
        if (relay == null)
            relay = obj.AddComponent<MobileTaggedButtonRelay>();

        relay.Initialize(tag);
        state.relay = relay;
    }

    public static bool GetButton(string tag)
    {
        ButtonState state = GetState(tag);
        return state != null && state.held;
    }

    public static bool GetButtonDown(string tag)
    {
        ButtonState state = GetState(tag);
        return state != null && state.downFrame == Time.frameCount;
    }

    public static bool GetButtonUp(string tag)
    {
        ButtonState state = GetState(tag);
        return state != null && state.upFrame == Time.frameCount;
    }

    internal static void Press(string tag)
    {
        ButtonState state = GetState(tag);
        if (state == null)
            return;

        if (!state.held)
            state.downFrame = Time.frameCount;

        state.held = true;
    }

    internal static void Release(string tag)
    {
        ButtonState state = GetState(tag);
        if (state == null)
            return;

        if (state.held)
            state.upFrame = Time.frameCount;

        state.held = false;
    }

    internal static void ClearRelay(string tag, MobileTaggedButtonRelay relay)
    {
        ButtonState state = GetState(tag);
        if (state == null)
            return;

        if (state.relay == relay)
            state.relay = null;

        state.held = false;
    }
}

public class MobileTaggedButtonRelay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private string buttonTag;

    public void Initialize(string tag)
    {
        buttonTag = tag;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileTaggedButtonInput.Press(buttonTag);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MobileTaggedButtonInput.Release(buttonTag);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MobileTaggedButtonInput.Release(buttonTag);
    }

    private void OnDisable()
    {
        MobileTaggedButtonInput.Release(buttonTag);
    }

    private void OnDestroy()
    {
        MobileTaggedButtonInput.ClearRelay(buttonTag, this);
    }
}
