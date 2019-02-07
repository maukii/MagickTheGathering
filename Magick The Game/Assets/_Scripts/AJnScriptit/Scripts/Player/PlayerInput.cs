using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region VARIABLES

    private bool inputEnabled               = false;
    private PlayerMovement playerMovement   = null;
    private ThirdPersonCamera tpCamera      = null;
    private PlayerShoot shoot               = null;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        playerMovement  = GetComponent<PlayerMovement>();
        tpCamera        = GetComponent<ThirdPersonCamera>();
        shoot           = GetComponent<PlayerShoot>();
    }

    void Update()
    {
        if (inputEnabled)
        {
            playerMovement.SetInput("Move", new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            playerMovement.SetInput("Jump", Input.GetButtonDown("Jump"));
            playerMovement.SetInput("Dash", Input.GetButtonDown("Fire3"));
            tpCamera.LookAround(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (Input.GetButtonDown("Fire1"))
            {
                shoot.ShootProjectile();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                tpCamera.SwitchSide();
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GetComponent<PlayerCore>().PauseGame();
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void EnableControls(bool b) { inputEnabled = b; }

    #endregion
}
