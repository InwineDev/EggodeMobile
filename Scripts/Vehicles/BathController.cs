using Mirror;
using UnityEngine;

public class BathController : NetworkBehaviour
{
    private Rigidbody rigidBody;
    [SerializeField] private float responsiveness = 10f;
    [SerializeField] private float yawResponsiveness = 10f;
    [SerializeField] private float throttleAmt = 25f;
    [SerializeField] private float maxThrust = 1f;
    private float throttle;

    private float roll;
    private float pitch;
    private float yaw;

    [Header("Damping (Stabilization)")]
    [SerializeField] private float rotationDamping = 5f; // Сила замедления вращения
    [SerializeField] private float maxAngularVelocity = 2f; // Максимальная скорость вращения
    [SerializeField] private float verticalDamping = 3f; // Замедление по вертикали

    [Header("Rotor")]
    [SerializeField] private float rotorSpeedModifier = 0.1f;
    [SerializeField] private Transform rotorTransform;

    private AudioSource audioSource;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<NetworkIdentity>().RemoveClientAuthority();
        rigidBody.maxAngularVelocity = maxAngularVelocity;
    }

    private void Update()
    {
        if (isOwned)
        {
            HandleUpdate();
            rotorTransform.Rotate(Vector3.up * (throttle * rotorSpeedModifier * Time.deltaTime * 100f));
        }
        audioSource.volume = Mathf.Clamp01(throttle * 0.01f);
    }

    private void FixedUpdate()
    {
        if (isOwned)
        {
            // ТЯГА (работает только при зажатом Space)
            if (Input.GetKey(KeyCode.Space))
            {
                rigidBody.AddForce(transform.up * (throttle * maxThrust), ForceMode.Force);
            }
            else
            {
                // Замедление по вертикали, если Space отпущен
                rigidBody.velocity = new Vector3(
                    rigidBody.velocity.x,
                    rigidBody.velocity.y * (1f - verticalDamping * Time.fixedDeltaTime),
                    rigidBody.velocity.z
                );
            }

            // ПОВОРОТЫ (исправлены оси)
            rigidBody.AddTorque(transform.right * pitch * responsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(-transform.forward * roll * responsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(transform.up * yaw * yawResponsiveness * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // ДЕМПФИРОВАНИЕ ВРАЩЕНИЯ (останавливает вращение)
            rigidBody.angularVelocity *= (1f - rotationDamping * Time.fixedDeltaTime);
        }
    }

    private void HandleUpdate()
    {
        // ИСПРАВЛЕННЫЕ ОСИ УПРАВЛЕНИЯ:
        // A = влево, D = вправо (крен)
        roll = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        // W = наклон вперед, S = наклон назад (тангаж)
        pitch = Input.GetKey(KeyCode.W) ? 0.1f : Input.GetKey(KeyCode.S) ? -0.1f : 0f;

        // Q/E = рыскание (горизонтальный поворот)
        yaw = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        // ТЯГА (набирается только при зажатом Space, сбрасывается при отпускании)
        if (Input.GetKey(KeyCode.Space))
        {
            throttle += throttleAmt * Time.deltaTime;
        }
        else
        {
            throttle -= throttleAmt * Time.deltaTime * 2f; // Быстрее сбрасываем тягу
        }
        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }
}