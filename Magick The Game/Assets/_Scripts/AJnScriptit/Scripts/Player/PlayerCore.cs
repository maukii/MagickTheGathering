using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ThirdPersonCamera))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCore : MonoBehaviour
{
    #region VARIABLES
    
    private Health cHealth                              = null;
    private ThirdPersonCamera cTPCamera                 = null;
    private PlayerInput cPlayerInput                    = null;
    private PlayerMovement cPlayerMovement              = null;
    private PlayerShoot cPlayerShoot                    = null;

    [Header("Input")]
    [SerializeField] private bool inputEnabled          = true;
    private bool inputEnabledTemp                       = true;
    [SerializeField] private GameObject crosshair       = null;

    [Header("Health")]
    [SerializeField] private float healthAmount         = 100.0f;
    [SerializeField] private float iFrameTime           = 1.0f;

    [Header("Third Person Camera")]
    [SerializeField] private bool invertY               = false;
    [SerializeField] private Vector2 sensitivity        = new Vector2(2.0f, 2.0f);
    [SerializeField] private Vector2 minMaxPitch        = new Vector2(-85.0f, 85.0f);
    [SerializeField] private Transform cameraPivot      = null;

    [Header("Movement")]
    [SerializeField] private float acceleration         = 200.0f;
    [SerializeField] private float airAcceleration      = 20.0f;
    [SerializeField] private float friction             = 0.2f;
    [SerializeField] private float airFriction          = 0.02f;
    [SerializeField] private float gravity              = -9.81f;
    [SerializeField] private float smoothStepDown       = 0.5f;
    [SerializeField] private float jumpForce            = 15.0f;
    [SerializeField] private float jumpGraceTime        = 0.1f;
    [SerializeField] private float dashSpeed            = 10.0f;
    [SerializeField] private float dashJumpForce        = 5.0f;
    [SerializeField] private float dashDuration         = 1.0f;
    [SerializeField] private float dashCooldown         = 2.0f;

    [Header("Physics")]
    [SerializeField] private LayerMask physicsLayerMask = 1;

    [Header("UI")]
    [SerializeField] private PauseMenu cPauseMenu       = null;
    [SerializeField] private GameOverMenu cGameOverMenu = null;
    [SerializeField] private HurtFlash cHurtFlash       = null;

    [Header("Model / Animations")]
    [SerializeField] private GameObject playerModel     = null;
    private bool isDead = false;

    #endregion

    #region VARIABLE_PROPERTIES

    //Input
    public bool InputEnabled { get { return inputEnabled; } }

    //Health
    public Health HealthComponent { get { return cHealth; } }
    public float Health { get { return healthAmount; } }
    public float IFrames { get { return iFrameTime; } }

    //Third Person Camera
    public ThirdPersonCamera TpcComponent { get { return cTPCamera; } }
    public bool InvertVertical { get { return invertY; } }
    public Vector2 Sensitivity { get { return sensitivity; } }
    public Transform CameraPivot { get { return cameraPivot; } }

    //Movement
    public float Acceleration { get { return acceleration; } }
    public float AirAcceleration { get { return airAcceleration; } }
    public float Friction { get { return friction; } }
    public float AirFriction { get { return airFriction; } }
    public float Gravity { get { return gravity; } }
    public float SmoothStepDown { get { return smoothStepDown; } }
    public float JumpForce { get { return jumpForce; } }
    public float JumpGraceTime { get { return jumpGraceTime; } }
    public float DashSpeed { get { return dashSpeed; } }
    public float DashJumpForce { get { return dashJumpForce; } }
    public float DashDuration { get { return dashDuration; } }
    public float DashCooldown { get { return dashCooldown; } }

    //Physics
    public LayerMask PhysicsLayerMask { get { return physicsLayerMask; } }

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Awake()
    {
        GlobalVariables.player = this;

        cHealth         = GetComponent<Health>();
        cTPCamera       = GetComponent<ThirdPersonCamera>();
        cPlayerInput    = GetComponent<PlayerInput>();
        cPlayerMovement = GetComponent<PlayerMovement>();
        cPlayerShoot    = GetComponent<PlayerShoot>();
    }

    void Update()
    {
        if (inputEnabledTemp != inputEnabled)
        {
            EnableControls(inputEnabled);
            inputEnabledTemp = inputEnabled;
        }

        if (isDead)
        {
            Camera.main.transform.LookAt(playerModel.transform);
        }
    }

    void OnEnable()
    {
        if (inputEnabled)
        {
            EnableControls(true);
        }
    }

    void OnDisable()
    {
        EnableControls(false);
    }

    #endregion

    #region CUSTOM_METHODS

    public void EnableControls(bool b)
    {
        Cursor.lockState = b ?
            CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !b;

        inputEnabled = b;
        cPlayerMovement.enableMovement = b;
        cPlayerInput.EnableControls(b);

        if (crosshair != null)
        {
            crosshair.SetActive(b);
        }
    }

    public void PauseGame()
    {
        EnableControls(!cPauseMenu.FlipPauseState(this));
    }

    public void Hurt()
    {
        if (cHurtFlash != null)
        {
            cHurtFlash.Flash();
        }
    }

    public void Dead()
    {
        if (cGameOverMenu != null)
        {
            EnableControls(false);
            cGameOverMenu.Activate();
        }

        if (playerModel != null)
        {
            isDead = true;
            playerModel.GetComponent<PlayerModelRotator>().enabled = false;
            playerModel.GetComponent<Rigidbody>().isKinematic = false;
            playerModel.GetComponent<CapsuleCollider>().enabled = true;
            playerModel.transform.SetParent(null);
            playerModel.GetComponent<Rigidbody>().AddForce(playerModel.transform.forward + playerModel.transform.up * -2.0f, ForceMode.VelocityChange);
        }
    }

    #endregion
}
