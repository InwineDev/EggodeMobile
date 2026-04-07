
using System.Reflection;
using UnityEngine;

public class MobileJoystickBridge : MonoBehaviour
{
    [SerializeField] private Component joystickSource;

    private static MobileJoystickBridge instance;
    private Component resolvedSource;

    private void Awake()
    {
        instance = this;
        resolvedSource = joystickSource != null ? joystickSource : ResolveSource();
    }

    public static float Horizontal => instance != null ? instance.ReadAxis("Horizontal") : 0f;
    public static float Vertical => instance != null ? instance.ReadAxis("Vertical") : 0f;

    private Component ResolveSource()
    {
        if (joystickSource != null)
            return joystickSource;

        GameObject tagged = GameObject.FindGameObjectWithTag("Joystick");
        if (tagged == null)
            return null;

        foreach (var component in tagged.GetComponents<Component>())
        {
            if (component == null || component is Transform || component is MobileJoystickBridge)
                continue;

            var type = component.GetType();
            if (type.GetProperty("Horizontal") != null && type.GetProperty("Vertical") != null)
                return component;
            if (type.GetField("Horizontal") != null && type.GetField("Vertical") != null)
                return component;
        }

        return null;
    }

    private float ReadAxis(string axisName)
    {
        if (resolvedSource == null)
            resolvedSource = ResolveSource();

        if (resolvedSource == null)
            return 0f;

        var type = resolvedSource.GetType();

        PropertyInfo property = type.GetProperty(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
            return System.Convert.ToSingle(property.GetValue(resolvedSource));

        FieldInfo field = type.GetField(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
            return System.Convert.ToSingle(field.GetValue(resolvedSource));

        return 0f;
    }
}
