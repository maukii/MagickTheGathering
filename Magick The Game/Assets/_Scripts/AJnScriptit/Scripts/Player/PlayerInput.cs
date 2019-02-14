using UnityEngine;

[RequireComponent(typeof(PlayerCore))]
public class PlayerInput : MonoBehaviour
{
    #region VARIABLES

    private bool inputEnabled               = false;
    private bool shotFired                  = false;
    private PlayerMovement playerMovement   = null;
    private ThirdPersonCamera tpCamera      = null;
    private PlayerSpellCaster shoot               = null;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        playerMovement  = GetComponent<PlayerMovement>();
        tpCamera        = GetComponent<ThirdPersonCamera>();
        shoot           = GetComponent<PlayerSpellCaster>();
    }

    void Update()
    {
        if (inputEnabled)
        {
            tpCamera.Look(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            playerMovement.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Jump"), Input.GetButtonDown("Fire3"));
            
            if (Input.GetButtonDown("Fire1") || Input.GetAxisRaw("Fire1") != 0.0f)
            {
                //Don't allow repeated input from controller axis
                if (!shotFired)
                {
                    shoot.CastSpell();
                    shotFired = true;
                }
            }
            else
            {
                shotFired = false;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                tpCamera.SwitchSide();
            }
        }

        if (Input.GetButtonDown("Escape"))
        {
            GetComponent<PlayerCore>().PauseGame();
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void EnableControls(bool b) { inputEnabled = b; }

    #endregion
}
