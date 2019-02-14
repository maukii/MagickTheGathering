using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ThirdPersonCamera))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerSpellCaster))]
public class PlayerCore : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private GameObject crosshair               = null;
    [SerializeField] private PauseMenu cPauseMenu               = null;
    [SerializeField] private GameOverMenu cGameOverMenu         = null;
    [SerializeField] private HurtFlash cHurtFlash               = null;
    [SerializeField] private GameObject playerModel             = null;

    public ThirdPersonCamera cTPCamera { get; private set; }    = null;
    public CharacterController cCharacter { get; private set; } = null;
    public LayerMask physicsLayerMask { get; private set; }     = 1;

    private bool bInputEnabled                                  = true;
    private bool bIsDead                                        = false;
    private bool shotFired = false;
    private Health cHealth                                      = null;
    private PlayerMovement cMovement                            = null;
    private PlayerSpellCaster cSpellCaster                            = null;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Awake()
    {
        GlobalVariables.player = this;

        cHealth         = GetComponent<Health>();
        cTPCamera       = GetComponent<ThirdPersonCamera>();
        cMovement       = GetComponent<PlayerMovement>();
        cCharacter      = GetComponent<CharacterController>();
        cSpellCaster    = GetComponent<PlayerSpellCaster>();
    }

    void Update()
    {
        if (bIsDead)
        {
            Camera.main.transform.LookAt(playerModel.transform);
        }
        else
        {
            if (bInputEnabled)
            {
                cTPCamera.Look(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                cMovement.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Jump"), Input.GetButtonDown("Fire3"));

                if (Input.GetButtonDown("Fire1") || Input.GetAxisRaw("Fire1") != 0.0f)
                {
                    //Don't allow repeated input from controller axis
                    if (!shotFired)
                    {
                        cSpellCaster.CastSpell();
                        shotFired = true;
                    }
                }
                else
                {
                    shotFired = false;
                }

                if (Input.GetButtonDown("Fire2"))
                {
                    cTPCamera.SwitchSide();
                }
            }

            if (Input.GetButtonDown("Escape"))
            {
                GetComponent<PlayerCore>().PauseGame();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "TriggerKill")
        {
            if (other.GetComponent<TriggerHurt>().killInstantly)
            {
                cHealth.Kill();
            }
            else
            {
                cHealth.Hurt(other.GetComponent<TriggerHurt>().damage);
            }
        }
    }

    void OnEnable()
    {
        if (bInputEnabled)
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

        bInputEnabled = b;
        cSpellCaster.CastBeamActive(b);

        if (crosshair != null)
        {
            crosshair.SetActive(b);
        }
    }

    public void PauseGame()
    {
        EnableControls(!cPauseMenu.FlipPauseState(this));
    }

    public void OnHurt()
    {
        if (cHurtFlash != null)
        {
            cHurtFlash.Flash();
        }
    }

    public void OnDeath()
    {
        if (cGameOverMenu != null)
        {
            EnableControls(false);
            cGameOverMenu.Activate();
        }

        if (playerModel != null)
        {
            bIsDead = true;
            playerModel.GetComponent<PlayerModelRotator>().enabled = false;
            playerModel.GetComponent<Rigidbody>().isKinematic = false;
            playerModel.GetComponent<CapsuleCollider>().enabled = true;
            playerModel.transform.SetParent(null);
            playerModel.GetComponent<Rigidbody>().AddForce(playerModel.transform.forward + playerModel.transform.up * -2.0f, ForceMode.VelocityChange);
        }
    }

    #endregion
}
