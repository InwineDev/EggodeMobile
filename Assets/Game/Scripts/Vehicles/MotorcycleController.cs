using System.Reflection;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotorcycleController : NetworkBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private float minEnginePitch = 0.8f;
    [SerializeField] private float maxEnginePitch = 1.5f;

    [Header("Wheels")]
    [SerializeField] private WheelCollider frontWheel;
    [SerializeField] private WheelCollider rearWheel;
    [SerializeField] private Transform frontWheelVisual;
    [SerializeField] private Transform rearWheelVisual;

    [Header("Driving")]
    [SerializeField] private float maxSpeed = 160f;
    [SerializeField] private float enginePower = 120f;
    [SerializeField] private float brakePower = 200f;
    [SerializeField] private float maxSteerAngle = 20f;
    [SerializeField] private string joystickTag = "Joystick";

    [Header("Physics")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.7f, 0f);
    [SerializeField] private float downForce = 40f;
    [SerializeField] private float steeringSmoothTime = 0.12f;
    [SerializeField] private float maxAngularVelocity = 2f;
    [SerializeField] private float stabilizationForce = 5f;
    [SerializeField] private float wheelAlignmentForce = 10f;
    [SerializeField] private float autoStraightenSpeed = 1.5f;

    [Header("Lean")]
    [SerializeField, Range(0, 60)] private float leanAngle = 25f;
    [SerializeField] private float leanSmoothTime = 0.15f;

    private Rigidbody rb;
    private Component joystickComponent;
    private float currentSpeed;
    private float inputSteer;
    private float inputThrottle;
    private bool isBraking;
    private float smoothedSteer;
    private float steerSmoothVelocity;
    private float leanSmoothVelocity;
    private float targetLean;
    private float steeringBalance;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMassOffset;
        rb.maxAngularVelocity = maxAngularVelocity;
        GetComponent<NetworkIdentity>().RemoveClientAuthority();
        joystickComponent = ResolveJoystick();
        ConfigureWheelFriction();
    }

    private void Update()
    {
        if (!isOwned)
            return;

        GetInput();
        UpdateWheelVisuals();
        CalculateLean();
        HandleEngineSound();
    }

    private void FixedUpdate()
    {
        if (!isOwned)
            return;

        ApplyEngineForce();
        ApplySteering();
        ApplyBrakes();
        ApplyDownForce();
        ApplyLean();
        LimitMaximumSpeed();
        Stabilize();
        AlignWheels();
    }

    private void GetInput()
    {
        Vector2 input = ReadJoystick();
        inputSteer = Mathf.Clamp(input.x, -1f, 1f);
        inputThrottle = Mathf.Clamp(input.y, -1f, 1f);
        isBraking = inputThrottle < -0.1f;
    }

    private void ApplyEngineForce()
    {
        currentSpeed = rb.velocity.magnitude * 3.6f;

        float motorInput = Mathf.Max(0f, inputThrottle);
        if (currentSpeed < maxSpeed)
        {
            float speedFactor = Mathf.SmoothStep(0f, 1f, 1f - (currentSpeed / maxSpeed));
            rearWheel.motorTorque = enginePower * motorInput * speedFactor;
        }
        else
        {
            rearWheel.motorTorque = 0f;
        }
    }

    private void ApplyBrakes()
    {
        float targetRearBrake = isBraking ? brakePower * Mathf.Abs(inputThrottle) : 0f;
        float targetFrontBrake = isBraking ? brakePower * 0.6f * Mathf.Abs(inputThrottle) : 0f;

        rearWheel.brakeTorque = Mathf.Lerp(rearWheel.brakeTorque, targetRearBrake, Time.fixedDeltaTime * 10f);
        frontWheel.brakeTorque = Mathf.Lerp(frontWheel.brakeTorque, targetFrontBrake, Time.fixedDeltaTime * 10f);
    }

    private void ApplySteering()
    {
        float speedFactor = Mathf.Lerp(1f, 0.4f, currentSpeed / maxSpeed);
        float targetSteer = inputSteer * maxSteerAngle * speedFactor;
        targetSteer += -steeringBalance * 0.1f;

        smoothedSteer = Mathf.SmoothDamp(smoothedSteer, targetSteer, ref steerSmoothVelocity, steeringSmoothTime);
        frontWheel.steerAngle = smoothedSteer;
        steeringBalance = Mathf.Lerp(steeringBalance, inputSteer, Time.fixedDeltaTime * 2f);
    }

    private void CalculateLean()
    {
        float speedFactor = Mathf.Clamp01(currentSpeed / 30f);
        targetLean = -smoothedSteer * leanAngle * speedFactor;
    }

    private void ApplyLean()
    {
        float currentLean = transform.localEulerAngles.z;
        if (currentLean > 180f)
            currentLean -= 360f;

        float smoothedLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanSmoothVelocity, leanSmoothTime);
        Vector3 euler = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(euler.x, euler.y, smoothedLean);
    }

    private void Stabilize()
    {
        float currentLean = transform.localEulerAngles.z;
        if (currentLean > 180f)
            currentLean -= 360f;

        if (Mathf.Abs(currentLean) > 2f)
        {
            float stabilizationTorque = -currentLean * stabilizationForce * rb.mass;
            rb.AddRelativeTorque(0f, 0f, stabilizationTorque * Time.fixedDeltaTime);
        }

        float sideStabilization = -rb.angularVelocity.y * 2f;
        rb.AddRelativeTorque(0f, sideStabilization, 0f);
    }

    private void AlignWheels()
    {
        if (Mathf.Abs(inputSteer) < 0.1f && currentSpeed > 5f)
        {
            float alignmentTorque = -rb.angularVelocity.y * wheelAlignmentForce;
            rb.AddTorque(0f, alignmentTorque, 0f);
            frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle, 0f, Time.fixedDeltaTime * autoStraightenSpeed);
        }
    }

    private void ApplyDownForce()
    {
        float speedFactor = Mathf.Pow(Mathf.Clamp01(currentSpeed / maxSpeed), 2f);
        rb.AddForce(-transform.up * downForce * (1f + speedFactor * 3f));
    }

    private void LimitMaximumSpeed()
    {
        float maxVelocity = maxSpeed / 3.6f;
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * maxVelocity, Time.fixedDeltaTime * 3f);
        }
    }

    private void HandleEngineSound()
    {
        if (engineAudioSource == null)
            return;

        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
        float targetPitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedRatio);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 3f);
        engineAudioSource.volume = Mathf.Lerp(0.2f, 1f, Mathf.Max(speedRatio, Mathf.Abs(inputThrottle)));
    }

    private void ConfigureWheelFriction()
    {
        if (frontWheel == null || rearWheel == null)
            return;

        WheelFrictionCurve frontForward = frontWheel.forwardFriction;
        frontForward.stiffness = 1.3f;
        frontForward.extremumSlip = 0.3f;
        frontWheel.forwardFriction = frontForward;

        WheelFrictionCurve frontSide = frontWheel.sidewaysFriction;
        frontSide.stiffness = 1.1f;
        frontSide.extremumSlip = 0.15f;
        frontWheel.sidewaysFriction = frontSide;

        WheelFrictionCurve rearForward = rearWheel.forwardFriction;
        rearForward.stiffness = 1.7f;
        rearForward.extremumSlip = 0.25f;
        rearWheel.forwardFriction = rearForward;

        WheelFrictionCurve rearSide = rearWheel.sidewaysFriction;
        rearSide.stiffness = 1.4f;
        rearSide.extremumSlip = 0.2f;
        rearWheel.sidewaysFriction = rearSide;
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheelVisual(frontWheel, frontWheelVisual);
        UpdateWheelVisual(rearWheel, rearWheelVisual);
    }

    private void UpdateWheelVisual(WheelCollider collider, Transform visual)
    {
        if (collider == null || visual == null)
            return;

        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        visual.position = position;
        visual.rotation = rotation;
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
