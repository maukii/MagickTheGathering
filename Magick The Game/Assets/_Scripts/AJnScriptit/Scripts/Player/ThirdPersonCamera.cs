using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    #region VARIABLES

    private bool invertY            = false;
    private Vector2 sensitivity     = new Vector2(1.0f, 1.0f);
    private Vector2 minMaxPitch     = new Vector2(-85.0f, 85.0f);
    private Transform cameraPivot   = null;
    private Vector3 lookDirection   = Vector3.zero;

    #endregion

    #region VARIABLE_PROPERTIES

    public Vector3 LookDirection { get { return lookDirection; } }

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        if (GetComponent<PlayerCore>() != null)
        {
            invertY = GetComponent<PlayerCore>().InvertVertical;
            sensitivity = GetComponent<PlayerCore>().Sensitivity;
            cameraPivot = GetComponent<PlayerCore>().CameraPivot;
        }
        else
        {
            Debug.LogError(this + " couldn't find player core!");
            this.enabled = false;
        }

        if (cameraPivot == null)
        {
            Debug.LogWarning(this + " is missing a camera pivot!");
            this.enabled = false;
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void LookAround(float x, float y)
    {
        lookDirection.x += y * sensitivity.x * (invertY ? 1.0f : -1.0f);
        lookDirection.y += x * sensitivity.y;

        if (lookDirection.x < minMaxPitch.x) { lookDirection.x = minMaxPitch.x; }
        if (lookDirection.x > minMaxPitch.y) { lookDirection.x = minMaxPitch.y; }

        cameraPivot.localRotation = Quaternion.Euler(lookDirection);
    }

    public void SwitchSide()
    {
        Transform camera = cameraPivot.GetChild(0);
        Vector3 camPos = camera.localPosition;
        camPos.x *= -1;
        camera.localPosition = camPos;
    }

    #endregion
}
