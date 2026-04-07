using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FirstPersonController : NetworkBehaviour
{
    public Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public GameObject[] canvasi;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    public bool lockCursor = false;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public bool isGrounded = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    [Header("Footstep Sounds")]
    public bool enableFootsteps = true;
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepIntervalWalk = 0.5f;
    public float footstepIntervalRun = 0.3f;
    public float footstepVolume = 0.5f;
    public float crouchFootstepVolumeMultiplier = 0.5f;
    public Animator animations;
    private float footstepTimer = 0;

    [SyncVar] private bool syncIsWalking;
    [SyncVar] private bool syncIsSprinting;
    [SyncVar] private bool syncIsCrouched;
    [SyncVar] private bool syncIsJumping;

    #region Mobile Controls

    [Header("Mobile Controls")]
    public bool enableMobileControls = true;
    public bool autoEnableMobileOnHandheld = true;
    public bool forceMobileInEditor = true;

    [Header("Mobile Movement")]
    public Joystick moveJoystick;
    public GameObject mobileControlsRoot;

    [Header("Mobile Look")]
    public MobileLookArea mobileLookArea;
    public bool useMobileLookArea = true;
    public float mobileLookMultiplier = 0.05f;

    private bool mobileJumpQueued;
    private bool mobileSprintToggled;
    private bool mobileCrouchHeld;
    private bool mobileZoomHeld;
    private bool mobileCrouchToggleQueued;

    #endregion

    public GameObject caPSULE;
    private GameObject cam;
    public GameObject canvaser;
    public GameObject canvasermama;
    public GameObject nick;
    public GameObject canvasnaya;
    public KeyCode destroyKey = KeyCode.Escape;
    public bool escaped = false;

    float camRotation;
    public userSettings us;
    private bool isSwimming;
    public GameObject waterVolume;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerCamera != null)
            playerCamera.fieldOfView = fov;

        originalScale = transform.localScale;

        if (joint != null)
            jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        ApplyCursorState();

        if (crosshairObject != null)
        {
            if (crosshair)
            {
                crosshairObject.sprite = crosshairImage;
                crosshairObject.color = crosshairColor;
            }
            else
            {
                crosshairObject.gameObject.SetActive(false);
            }
        }

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar && sprintBarBG != null && sprintBar != null)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull && sprintBarCG != null)
                sprintBarCG.alpha = 0;
        }
        else
        {
            if (sprintBarBG != null) sprintBarBG.gameObject.SetActive(false);
            if (sprintBar != null) sprintBar.gameObject.SetActive(false);
        }

        RefreshMobileUI();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        RefreshMobileUI();
        ApplyCursorState();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer && playerCamera != null)
            playerCamera.gameObject.SetActive(false);
    }

    private void RefreshMobileUI()
    {
        if (mobileControlsRoot == null) return;

        if (isLocalPlayer)
            mobileControlsRoot.SetActive(UseMobileInput());
    }

    private bool UseMobileInput()
    {
        if (!enableMobileControls) return false;

        if (Application.isMobilePlatform && autoEnableMobileOnHandheld)
            return true;

#if UNITY_EDITOR
        if (forceMobileInEditor)
            return true;
#endif

        return false;
    }

    private void ApplyCursorState()
    {
        if (UseMobileInput())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (lockCursor && !escaped)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            isSwimming = true;
            if (rb != null) rb.useGravity = false;
            if (waterVolume != null) waterVolume.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            isSwimming = false;
            if (rb != null) rb.useGravity = true;
            if (waterVolume != null) waterVolume.SetActive(false);
        }
    }

    private void Update()
    {
        if (!escaped)
        {
            if (isLocalPlayer)
            {
                if (cameraCanMove && playerCamera != null)
                {
                    Vector2 lookInput = GetLookInput();

                    yaw = transform.localEulerAngles.y + lookInput.x;

                    bool invertY = invertCamera;
                    if (!invertY)
                        pitch -= lookInput.y;
                    else
                        pitch += lookInput.y;

                    pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

                    transform.localEulerAngles = new Vector3(0, yaw, 0);
                    playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
                }

                if (enableZoom && playerCamera != null)
                {
                    if (!holdToZoom && !isSprinting)
                    {
                        if (Input.GetKeyDown(zoomKey))
                            isZoomed = !isZoomed;
                    }

                    if (holdToZoom && !isSprinting)
                    {
                        bool zoomHeldNow = Input.GetKey(zoomKey) || mobileZoomHeld;
                        isZoomed = zoomHeldNow;
                    }

                    if (isZoomed)
                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
                    else if (!isZoomed && !isSprinting)
                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
                }

                if (enableSprint)
                {
                    if (isSprinting)
                    {
                        isZoomed = false;

                        if (playerCamera != null)
                            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                        if (!unlimitedSprint)
                        {
                            sprintRemaining -= 1 * Time.deltaTime;
                            if (sprintRemaining <= 0)
                            {
                                isSprinting = false;
                                mobileSprintToggled = false;
                                isSprintCooldown = true;
                            }
                        }
                    }
                    else
                    {
                        sprintRemaining = Mathf.Clamp(sprintRemaining + Time.deltaTime, 0, sprintDuration);
                    }

                    if (isSprintCooldown)
                    {
                        sprintCooldown -= Time.deltaTime;
                        if (sprintCooldown <= 0)
                            isSprintCooldown = false;
                    }
                    else
                    {
                        sprintCooldown = sprintCooldownReset;
                    }

                    if (useSprintBar && !unlimitedSprint && sprintBar != null)
                    {
                        float sprintRemainingPercent = sprintRemaining / sprintDuration;
                        sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
                    }
                }

                if (enableJump && GetJumpPressed() && isGrounded)
                    Jump();

                if (enableCrouch)
                {
                    if (!holdToCrouch)
                    {
                        if (Input.GetKeyDown(crouchKey) || mobileCrouchToggleQueued)
                        {
                            mobileCrouchToggleQueued = false;
                            Crouch();
                        }
                    }
                    else
                    {
                        bool crouchPressed = Input.GetKey(crouchKey) || mobileCrouchHeld;

                        if (crouchPressed && !isCrouched)
                            Crouch();
                        else if (!crouchPressed && isCrouched)
                            Crouch();
                    }
                }
            }
        }

        if (isLocalPlayer && us != null && !us.canWrite)
        {
            if (Input.GetKeyDown(destroyKey))
                ToggleEscapeState();
        }

        if (enableFootsteps && isGrounded && isWalking)
            PlayFootstepSound();

        UpdateAnimations();
        CheckGround();

        if (enableHeadBob)
            HeadBob();
    }

    void FixedUpdate()
    {
        if (!escaped && isLocalPlayer)
        {
            if (playerCanMove && rb != null)
            {
                Vector2 moveInput = GetMoveInput();
                Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y);

                isWalking = (targetVelocity.x != 0 || targetVelocity.z != 0) && isGrounded;

                bool wantsSprint = enableSprint && GetSprintHeld() && sprintRemaining > 0f && !isSprintCooldown;

                if (wantsSprint)
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                    Vector3 velocity = rb.velocity;
                    Vector3 velocityChange = targetVelocity - velocity;
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    if (velocityChange.x != 0 || velocityChange.z != 0)
                    {
                        isSprinting = true;

                        if (isCrouched)
                            Crouch();

                        if (hideBarWhenFull && !unlimitedSprint && sprintBarCG != null)
                            sprintBarCG.alpha += 5 * Time.deltaTime;
                    }

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                else
                {
                    isSprinting = false;

                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                    Vector3 velocity = rb.velocity;
                    Vector3 velocityChange = targetVelocity - velocity;
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }
        }
    }

    private Vector2 GetMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (UseMobileInput() && moveJoystick != null)
        {
            x = moveJoystick.Horizontal;
            y = moveJoystick.Vertical;
        }

        Vector2 value = new Vector2(x, y);
        if (value.sqrMagnitude > 1f) value.Normalize();
        return value;
    }

    private Vector2 GetLookInput()
    {
        if (UseMobileInput())
        {
            if (useMobileLookArea && mobileLookArea != null)
                return mobileLookArea.lookDelta * mobileLookMultiplier;

            return Vector2.zero;
        }

        float x = Input.GetAxis("Mouse X") * mouseSensitivity;
        float y = Input.GetAxis("Mouse Y") * mouseSensitivity;
        return new Vector2(x, y);
    }

    private bool GetJumpPressed()
    {
        bool pressed = Input.GetKeyDown(jumpKey) || mobileJumpQueued;
        mobileJumpQueued = false;
        return pressed;
    }

    private bool GetSprintHeld()
    {
        if (UseMobileInput())
            return mobileSprintToggled;

        return Input.GetKey(sprintKey);
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f * transform.localScale.y;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void PlayFootstepSound()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (footstepSounds != null && footstepSounds.Length > 0 && footstepAudioSource != null)
            {
                int n = Random.Range(0, footstepSounds.Length);

                float volume = footstepVolume;
                if (isCrouched)
                    volume *= crouchFootstepVolumeMultiplier;

                CmdFootsteps(volume, n);
            }

            footstepTimer = isSprinting ? footstepIntervalRun : footstepIntervalWalk;
        }
    }

    [Command]
    void CmdFootsteps(float volume, int n)
    {
        RpcFootsteps(volume, n);
    }

    [ClientRpc]
    void RpcFootsteps(float volume, int n)
    {
        if (footstepAudioSource == null || footstepSounds == null || footstepSounds.Length == 0) return;
        if (n < 0 || n >= footstepSounds.Length) return;

        footstepAudioSource.volume = volume;
        footstepAudioSource.clip = footstepSounds[n];
        footstepAudioSource.Play();
    }

    private void Jump()
    {
        if (!escaped)
        {
            if (isGrounded && rb != null)
            {
                rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
                isGrounded = false;
            }

            if (isCrouched && !holdToCrouch)
                Crouch();
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(sprintKey) || mobileSprintToggled) return;

        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;
            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;
            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (joint == null) return;

        if (isWalking)
        {
            if (isSprinting)
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            else if (isCrouched)
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            else
                timer += Time.deltaTime * bobSpeed;

            joint.localPosition = new Vector3(
                jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x,
                jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y,
                jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z
            );
        }
        else
        {
            timer = 0;
            joint.localPosition = new Vector3(
                Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed)
            );
        }
    }

    private void UpdateAnimations()
    {
        if (!isLocalPlayer || animations == null) return;

        bool wasWalking = animations.GetBool("walk");
        bool nowWalking = isWalking && !isSprinting;
        bool wasSprinting = animations.GetBool("run");
        bool nowSprinting = isSprinting;

        if (wasWalking != nowWalking)
        {
            animations.SetBool("walk", nowWalking);
            CmdUpdateAnimationStates(nowWalking, isSprinting, isCrouched);
        }

        if (wasSprinting != nowSprinting)
        {
            animations.SetBool("run", nowSprinting);
            CmdUpdateAnimationStates(isWalking, nowSprinting, isCrouched);
        }
    }

    [Command]
    private void CmdUpdateAnimationStates(bool walking, bool sprinting, bool crouched)
    {
        syncIsWalking = walking;
        syncIsSprinting = sprinting;
        syncIsCrouched = crouched;
        RpcUpdateAnimationStates(walking, sprinting, crouched);
    }

    [ClientRpc]
    private void RpcUpdateAnimationStates(bool walking, bool sprinting, bool crouched)
    {
        if (isLocalPlayer || animations == null) return;

        animations.SetBool("walk", walking && !sprinting);
        animations.SetBool("run", sprinting);
    }

    private void ToggleEscapeState()
    {
        if (escaped)
        {
            userSettingNotCam usc = gameObject.GetComponent<userSettingNotCam>();
            if (usc != null) usc.one();

            escaped = false;
            isWalking = false;
        }
        else
        {
            userSettingNotCam usc = gameObject.GetComponent<userSettingNotCam>();
            if (usc != null) usc.two();

            escaped = true;
            isWalking = false;

            if (rb != null)
                rb.velocity = Vector3.zero;
        }

        ApplyCursorState();
    }

    #region Mobile Button Methods

    public void MobileJump()
    {
        mobileJumpQueued = true;
    }

    public void MobileSprintToggle()
    {
        mobileSprintToggled = !mobileSprintToggled;
    }

    public void MobileSprintSet(bool state)
    {
        mobileSprintToggled = state;
    }

    public void MobileZoomDown()
    {
        mobileZoomHeld = true;
    }

    public void MobileZoomUp()
    {
        mobileZoomHeld = false;
        if (holdToZoom)
            isZoomed = false;
    }

    public void MobileCrouchDown()
    {
        mobileCrouchHeld = true;
    }

    public void MobileCrouchUp()
    {
        mobileCrouchHeld = false;
    }

    public void MobileCrouchToggle()
    {
        mobileCrouchToggleQueued = true;
    }

    public void MobileEscapeToggle()
    {
        if (!isLocalPlayer) return;
        ToggleEscapeState();
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController))]
public class FirstPersonControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif