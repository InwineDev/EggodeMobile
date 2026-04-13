using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotorcycleController : NetworkBehaviour
{
    [Header("Audio Settings")]
    public AudioSource engineAudioSource;
    public float minEnginePitch = 0.8f;
    public float maxEnginePitch = 1.5f;
 
    [Header("Wheel Settings")]
    public WheelCollider frontWheel;
    public WheelCollider rearWheel;
    public Transform frontWheelVisual;
    public Transform rearWheelVisual;
  
    [Header("Performance Settings")]
    [SerializeField] private float maxSpeed = 160f;
    [SerializeField] private float enginePower = 120f;
    [SerializeField] private float brakePower = 150f;
    [SerializeField] private float maxSteerAngle = 15f; // Уменьшен угол поворота
    [SerializeField] private float boostMultiplier = 1.5f;

    [Header("Physics Settings")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0, -0.7f, 0); // Ниже центр масс
    [SerializeField] private float downForce = 40f;
    [SerializeField] private float steeringSmoothTime = 0.2f; // Увеличено время сглаживания

    private Rigidbody rb;
    private float currentSpeed;
    private float inputSteer;
    private float inputThrottle;
    private bool isBraking;
    private bool isBoosting;

    private float smoothedSteer;
    private float steerSmoothVelocity;
    private float originalEnginePower;
    private float originalMaxSpeed;

    [Header("Lean Settings")]
    [SerializeField][Range(0, 60)] private float leanAngle = 30f;
    [SerializeField][Range(0, 1)] private float brakeLeanReduction = 0.5f;
    [SerializeField] private float leanSmoothTime = 0.2f;

    private float leanSmoothVelocity;
    private float targetLean;

    [Header("Stabilization Settings")]
    [SerializeField] private float maxAngularVelocity = 2f;
    [SerializeField] private float stabilizationForce = 5f;
    [SerializeField] private float counterSteerFactor = 0.5f;

    [Header("Balance Settings")]
    [SerializeField] private float wheelAlignmentForce = 10f; // Сила выравнивания колес
    [SerializeField] private float autoStraightenSpeed = 1.5f; // Скорость авто-выравнивания

    private float steeringBalance; // Баланс поворота

    [Header("Auto Upright Settings")]
    [SerializeField] private float uprightCheckInterval = 0.2f;
    [SerializeField] private float uprightForce = 50f;
    [SerializeField] private float uprightOffset = 0.2f; // Смещение центра стабилизации вверх
    [SerializeField] private float uprightTorque = 30f;
    [SerializeField] private float uprightSpeedThreshold = 1f; // Минимальная скорость для стабилизации
    [SerializeField] private float uprightMaxAngle = 80f; // Максимальный угол для стабилизации

    private float lastUprightCheckTime = 0f;
    private bool isUprightStabilizing = false;

    [SerializeField] private SittingController vz;
    private void Start()
    {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = centerOfMassOffset;
            rb.maxAngularVelocity = maxAngularVelocity;
            GetComponent<NetworkIdentity>().RemoveClientAuthority();
            originalEnginePower = enginePower;
            originalMaxSpeed = maxSpeed;

            ConfigureWheelFriction();
    }

    private void Update()
    {
        CheckUprightState();
        if (/*vz.svoboda == 1 &*/ isOwned)
        {

            GetInput();
            UpdateWheelVisuals();
            CalculateLean();
            HandleEngineSound();
            HandleBoost();
        }
    }

    private void FixedUpdate()
    {
        if (isUprightStabilizing)
        {
            ApplyUprightForces();
        }
        if (/*vz.svoboda == 1 & */isOwned)
        {
            ApplyEngineForce();
            ApplySteering();
            ApplyBrakes();
            ApplyDownForce();
            ApplyLean();
            LimitMaximumSpeed();
            Stabilize();
            AlignWheels();
        }
    }

    private void CheckUprightState()
    {
        if (Time.time - lastUprightCheckTime > uprightCheckInterval)
        {
            lastUprightCheckTime = Time.time;

            // Проверяем угол наклона
            float angle = Vector3.Angle(transform.up, Vector3.up);

            // Проверяем скорость - не стабилизируем на высоких скоростях
            bool isMovingSlow = rb.velocity.magnitude < uprightSpeedThreshold;

            // Если угол слишком большой и скорость небольшая
            isUprightStabilizing = angle > uprightMaxAngle && isMovingSlow;
        }
    }

    private void ApplyUprightForces()
    {
        // 1. Сила, поднимающая мотоцикл
        Vector3 upliftDirection = Vector3.up;
        Vector3 upliftPosition = transform.position + transform.up * uprightOffset;
        rb.AddForceAtPosition(upliftDirection * uprightForce * rb.mass, upliftPosition);

        // 2. Крутящий момент для выравнивания
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * 0.6f,
            rb.angularVelocity
        ) * transform.up;

        float torqueFactor = Vector3.Dot(predictedUp, Vector3.up);
        Vector3 torqueDirection = Vector3.Cross(predictedUp, Vector3.up);

        rb.AddTorque(torqueDirection * uprightTorque * rb.mass * torqueFactor);

        // 3. Демпфирование вращения
        rb.AddTorque(-rb.angularVelocity * uprightTorque * 0.1f * rb.mass);
    }

    private void HandleBoost()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            enginePower = originalEnginePower * boostMultiplier;
            maxSpeed = originalMaxSpeed * boostMultiplier;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            enginePower = originalEnginePower;
            maxSpeed = originalMaxSpeed;
        }
    }

    private void GetInput()
    {
        inputThrottle = Mathf.Clamp(Mathf.Pow(Input.GetAxis("Vertical"), 2) * Mathf.Sign(Input.GetAxis("Vertical")), 0, 1);
        inputSteer = Mathf.Pow(Input.GetAxis("Horizontal"), 3);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleEngineSound()
    {
        float speedRatio = currentSpeed / maxSpeed;
        float targetPitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedRatio);

        engineAudioSource.pitch = Mathf.Lerp(
            engineAudioSource.pitch,
            targetPitch,
            Time.deltaTime * 3f
        );
    }

    private void ConfigureWheelFriction()
    {
        // Переднее колесо
        WheelFrictionCurve frontForward = frontWheel.forwardFriction;
        frontForward.stiffness = 1.3f;
        frontForward.extremumSlip = 0.3f;
        frontWheel.forwardFriction = frontForward;

        WheelFrictionCurve frontSide = frontWheel.sidewaysFriction;
        frontSide.stiffness = 1.1f;
        frontSide.extremumSlip = 0.15f;
        frontWheel.sidewaysFriction = frontSide;

        // Заднее колесо
        WheelFrictionCurve rearForward = rearWheel.forwardFriction;
        rearForward.stiffness = 1.7f;
        rearForward.extremumSlip = 0.25f;
        rearWheel.forwardFriction = rearForward;

        WheelFrictionCurve rearSide = rearWheel.sidewaysFriction;
        rearSide.stiffness = 1.4f;
        rearSide.extremumSlip = 0.2f;
        rearWheel.sidewaysFriction = rearSide;
    }

    private void ApplyEngineForce()
    {
        currentSpeed = rb.velocity.magnitude * 3.6f;

        if (currentSpeed < maxSpeed)
        {
            // Плавное нарастание мощности с S-образной кривой
            float speedFactor = Mathf.SmoothStep(0, 1, 1 - (currentSpeed / maxSpeed));
            rearWheel.motorTorque = enginePower * inputThrottle * speedFactor;
        }
        else
        {
            rearWheel.motorTorque = 0;
        }
    }

    private void ApplyBrakes()
    {
        if (isBraking)
        {
            // Плавное нарастание тормозного усилия
            float brakeFactor = Mathf.Lerp(0.3f, 1f, Mathf.Abs(inputThrottle));
            rearWheel.brakeTorque = brakePower * brakeFactor;
            frontWheel.brakeTorque = brakePower * 0.6f * brakeFactor;
        }
        else
        {
            // Плавное снятие тормозного усилия
            rearWheel.brakeTorque = Mathf.Lerp(rearWheel.brakeTorque, 0, Time.fixedDeltaTime * 5f);
            frontWheel.brakeTorque = Mathf.Lerp(frontWheel.brakeTorque, 0, Time.fixedDeltaTime * 5f);
        }
    }

    private void ApplySteering()
    {
        float speedFactor = Mathf.Lerp(1f, 0.4f, currentSpeed / maxSpeed);
        float targetSteer = inputSteer * maxSteerAngle * speedFactor;

        // Компенсация самопроизвольного поворота
        float balanceCompensation = -steeringBalance * 0.1f;
        targetSteer += balanceCompensation;

        smoothedSteer = Mathf.SmoothDamp(smoothedSteer, targetSteer,
            ref steerSmoothVelocity, steeringSmoothTime);

        frontWheel.steerAngle = smoothedSteer;

        // Обновляем баланс поворота
        steeringBalance = Mathf.Lerp(steeringBalance, inputSteer, Time.fixedDeltaTime * 2f);
    }

    private void Stabilize()
    {
        // Стабилизация по крену
        float currentLean = transform.rotation.eulerAngles.z;
        if (currentLean > 180) currentLean -= 360;

        // Вертикальная стабилизация
        if (Mathf.Abs(currentLean) > 2f)
        {
            float stabilizationTorque = -currentLean * stabilizationForce * rb.mass;
            rb.AddRelativeTorque(0, 0, stabilizationTorque * Time.fixedDeltaTime);
        }

        // Боковая стабилизация (против самопроизвольного поворота)
        float sideStabilization = -rb.angularVelocity.y * 2f;
        rb.AddRelativeTorque(0, sideStabilization, 0);
    }
    private void AlignWheels()
    {
        // Автоматическое выравнивание колес при отсутствии ввода
        if (Mathf.Abs(inputSteer) < 0.1f && currentSpeed > 5f)
        {
            float alignmentTorque = -rb.angularVelocity.y * wheelAlignmentForce;
            rb.AddTorque(0, alignmentTorque, 0);

            // Плавное возвращение руля в центр
            frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle, 0,
                Time.fixedDeltaTime * autoStraightenSpeed);
        }
    }

    private void CalculateLean()
    {
        // Целевой наклон зависит от поворота и скорости
        float speedFactor = Mathf.Clamp01(currentSpeed / 30f);
        targetLean = -smoothedSteer * leanAngle * speedFactor;

        // Учет торможения
        float maxLean = isBraking ? leanAngle * (1 - brakeLeanReduction) : leanAngle;
        targetLean = Mathf.Clamp(targetLean, -maxLean, maxLean);
    }

    private void ApplyLean()
    {
        // Плавный наклон с учетом скорости
        float currentLean = transform.rotation.eulerAngles.z;
        if (currentLean > 180) currentLean -= 360;

        float smoothedLean = Mathf.SmoothDamp(currentLean, targetLean,
                                           ref leanSmoothVelocity,
                                           leanSmoothTime * (1 + currentSpeed / maxSpeed));

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            smoothedLean
        );
    }

    private void ApplyDownForce()
    {
        // Прижимная сила растет квадратично со скоростью
        float speedFactor = Mathf.Pow(currentSpeed / maxSpeed, 2);
        rb.AddForce(-transform.up * downForce * (1 + speedFactor * 3f));
    }

    private void LimitMaximumSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed / 3.6f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity,
                                     rb.velocity.normalized * maxSpeed / 3.6f,
                                     Time.fixedDeltaTime * 3f);
        }
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheelVisual(frontWheel, frontWheelVisual);
        UpdateWheelVisual(rearWheel, rearWheelVisual);
    }

    private void UpdateWheelVisual(WheelCollider collider, Transform visual)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        visual.position = position;
        visual.rotation = rotation;
    }
}