using System.Reflection;
using Mirror;
using UnityEngine;

public class BathController : NetworkBehaviour
{
    private Rigidbody rigidBody;

    [Header("Flight")]
    [SerializeField] private float responsiveness = 10f;
    [SerializeField] private float yawResponsiveness = 10f;
    [SerializeField] private float liftForce = 25f;
    [SerializeField] private float moveForce = 12f;
    [SerializeField] private float maxHorizontalSpeed = 12f;
    [SerializeField] private string joystickTag = "Joystick";
    [SerializeField] private string jumpButtonTag = "JumpButton";

    [Header("Stabilization")]
    [SerializeField] private float rotationDamping = 4f;
    [SerializeField] private float maxAngularVelocity = 2f;
    [SerializeField] private float uprightTorque = 6f;

    [Header("Rotor")]
    [SerializeField] private float rotorIdleSpeed = 300f;
    [SerializeField] private float rotorFlightSpeed = 900f;
    [SerializeField] private float rotorAcceleration = 8f;
    [SerializeField] private Transform rotorTransform;

    private AudioSource audioSource;
    private float roll;
    private float pitch;
    private float yaw;
    private bool flyEnabled;
    private float rotorSpeed;
    private Component joystickComponent;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<NetworkIdentity>().RemoveClientAuthority();
        rigidBody.maxAngularVelocity = maxAngularVelocity;
        joystickComponent = ResolveJoystick();
    }

    private void Update()
    {
        if (!isOwned)
            return;

        HandleUpdate();
        UpdateRotor();
        UpdateAudio();
    }

    private void FixedUpdate()
    {
        if (!isOwned)
            return;

        ApplyMovement();
        ApplyRotation();
    }

    private void HandleUpdate()
    {
        Vector2 input = ReadJoystick();
        roll = Mathf.Clamp(input.x, -1f, 1f);
        yaw = roll;
        pitch = Mathf.Clamp(input.y, -1f, 1f);

        if (MobileTaggedInput.WasPressedThisFrame(jumpButtonTag))
            flyEnabled = !flyEnabled;
    }

    private void ApplyMovement()
    {
        Vector3 planarInput = (transform.forward * pitch + transform.right * roll);
        if (planarInput.sqrMagnitude > 1f)
            planarInput.Normalize();

        rigidBody.AddForce(planarInput * moveForce, ForceMode.Acceleration);

        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rigidBody.velocity, Vector3.up);
        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        {
            Vector3 limitedHorizontal = horizontalVelocity.normalized * maxHorizontalSpeed;
            rigidBody.velocity = new Vector3(limitedHorizontal.x, rigidBody.velocity.y, limitedHorizontal.z);
        }

        if (flyEnabled)
            rigidBody.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
    }

    private void ApplyRotation()
    {
        rigidBody.AddRelativeTorque(Vector3.right * pitch * responsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);
        rigidBody.AddRelativeTorque(Vector3.back * roll * responsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);
        rigidBody.AddRelativeTorque(Vector3.up * yaw * yawResponsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);

        Vector3 uprightAxis = Vector3.Cross(transform.up, Vector3.up);
        rigidBody.AddTorque(uprightAxis * uprightTorque, ForceMode.Acceleration);
        rigidBody.angularVelocity *= Mathf.Clamp01(1f - rotationDamping * Time.fixedDeltaTime);
    }

    private void UpdateRotor()
    {
        float targetRotorSpeed = flyEnabled ? rotorFlightSpeed : rotorIdleSpeed;
        rotorSpeed = Mathf.Lerp(rotorSpeed, targetRotorSpeed, Time.deltaTime * rotorAcceleration);

        if (rotorTransform != null)
            rotorTransform.Rotate(Vector3.up * rotorSpeed * Time.deltaTime, Space.Self);
    }

    private void UpdateAudio()
    {
        if (audioSource == null)
            return;

        float normalizedRotor = Mathf.InverseLerp(rotorIdleSpeed, rotorFlightSpeed, rotorSpeed);
        audioSource.volume = Mathf.Lerp(0.15f, 1f, normalizedRotor);
        audioSource.pitch = Mathf.Lerp(0.8f, 1.2f, normalizedRotor);
    }

    private Vector2 ReadJoystick()
    {
        if (joystickComponent == null)
            joystickComponent = ResolveJoystick();

        if (joystickComponent == null)
            return Vector2.zero;

        return new Vector2(ReadAxis(joystickComponent, "Horizontal"), ReadAxis(joystickComponent, "Vertical"));
    }

    private Component ResolveJoystick()
    {
        GameObject tagged;
        try
        {
            tagged = GameObject.FindGameObjectWithTag(joystickTag);
        }
        catch
        {
            return null;
        }

        if (tagged == null)
            return null;

        foreach (Component component in tagged.GetComponents<Component>())
        {
            if (component == null || component is Transform)
                continue;

            if (HasAxis(component, "Horizontal") && HasAxis(component, "Vertical"))
                return component;
        }

        return null;
    }

    private static bool HasAxis(Component component, string axisName)
    {
        System.Type type = component.GetType();
        return type.GetProperty(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null ||
               type.GetField(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
    }

    private static float ReadAxis(Component component, string axisName)
    {
        System.Type type = component.GetType();

        PropertyInfo property = type.GetProperty(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
            return System.Convert.ToSingle(property.GetValue(component, null));

        FieldInfo field = type.GetField(axisName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
            return System.Convert.ToSingle(field.GetValue(component));

        return 0f;
    }
}
