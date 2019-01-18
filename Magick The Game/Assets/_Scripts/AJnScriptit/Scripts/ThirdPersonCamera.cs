//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    #region VARIABLES

    public bool invertY = false;
    public Vector2 sensitivity = new Vector2(1.0f, 1.0f);
    public Transform lockToTarget;

    [SerializeField] private Vector2 minMaxPitch;
    [SerializeField] private float preferredDistance;
    [SerializeField] private Text debugText;

    private Vector3 rotation;
    private Vector3 mousePosition = new Vector3(0.0f, 0.0f, 0.0f);

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        if (lockToTarget != null)
        {
            rotation = new Vector3(0.0f, lockToTarget.rotation.y, 0.0f);
        }
        else
        {
            Debug.LogError(this.gameObject + " has no lock target attached to it!");
        }
    }

    void LateUpdate()
    {
        UpdateCameraPosition(CameraMovementDelta());
    }

    #endregion

    #region CUSTOM_METHODS

    Vector2 CameraMovementDelta()
    {
        Vector3 delta = Vector3.Scale(Input.mousePosition - mousePosition, (Vector3)sensitivity);
        mousePosition = Input.mousePosition;
        if (!invertY) { delta.y *= -1; }
        return delta;
    }

    void UpdateCameraPosition(Vector3 delta)
    {
        rotation.x += delta.y;
        rotation.y += delta.x;

        if (rotation.x < minMaxPitch.x)
        {
            rotation.x = minMaxPitch.x;
        }
        else if (rotation.x > minMaxPitch.y)
        {
            rotation.x = minMaxPitch.y;
        }
        
        lockToTarget.localRotation = Quaternion.Euler(rotation);
        debugText.text = rotation.ToString();
    }

    #endregion
}
