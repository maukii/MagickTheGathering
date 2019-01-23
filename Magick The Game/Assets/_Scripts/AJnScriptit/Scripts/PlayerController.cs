using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    public bool invertY = false;
    public Vector2 sensitivity = new Vector2(1.0f, 1.0f);

    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private float airAcceleration = 4.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Vector2 minMaxPitch = new Vector2(-85.0f, 85.0f);
    [SerializeField] private Transform cameraPivot = null;
    [SerializeField] private LayerMask physicsMask;

    private bool isGrounded = false;
    private bool isJumping = false;
    
    private Rigidbody rb;
    private Vector3 lookDirection = Vector3.zero;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraPivot == null)
        {
            Debug.LogWarning(this + " is missing a camera pivot reference!");
            this.enabled = false;
        }
    }

    void Update()
    {
        ActionLookAround(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        ActionJump(Input.GetButtonDown("Jump"));
    }

    //Movement is done in FixedUpdate as it depends on the rigidbody
    void FixedUpdate()
    {
        CheckIfGrounded();
        ActionMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ActionJump();
    }

    #endregion

    #region CUSTOM_METHODS

    void CheckIfGrounded()
    {
        //RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, /*out hit,*/ 0.1f, physicsMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ActionLookAround(float x, float y)
    {
        lookDirection.x += y * sensitivity.x * (invertY ? 1.0f : -1.0f);
        lookDirection.y += x * sensitivity.y;

        if (lookDirection.x < minMaxPitch.x) { lookDirection.x = minMaxPitch.x; }
        if (lookDirection.x > minMaxPitch.y) { lookDirection.x = minMaxPitch.y; }

        cameraPivot.localRotation = Quaternion.Euler(lookDirection);
    }

    void ActionMove(float x, float y)
    {
        Vector3 lookVector = new Vector3(Mathf.Sin(lookDirection.y * Mathf.Deg2Rad), 0.0f, Mathf.Cos(lookDirection.y * Mathf.Deg2Rad));
        Vector3 sideLookVector = Vector3.Cross(lookVector, Vector3.down);
        Vector3 rbVector = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        Vector3 combinedVectors = isGrounded ?
            combinedVectors = (lookVector * (acceleration * y)) + (sideLookVector * (acceleration * x))
            : combinedVectors = (lookVector * (airAcceleration * y)) + (sideLookVector * (airAcceleration * x));

        //Vector3 combinedVectors = Vector3.Normalize((lookVector * y) + (sideLookVector * x));
        //Vector3 rbVectorInverse = rbVector * -1;

        rb.AddForce(combinedVectors, ForceMode.Acceleration);

        Debug.DrawLine(transform.position, transform.position + lookVector, Color.blue);    //Forward vector
        Debug.DrawLine(transform.position, transform.position + sideLookVector, Color.red); //Right vector
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green);   //Up vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + combinedVectors, Color.cyan); //Desired movement velocity
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + combinedVectors.normalized, Color.yellow); //Desired movement unit vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rbVector, Color.magenta); //Rigidbody XZ velocity
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up * rb.velocity.y, Color.magenta); //Rigidbody Y velocity
    }

    //Jumping is split into "FixedUpdate" and "Update" to guarantee final execution during FixedUpdate
    void ActionJump()
    {
        if (isGrounded && isJumping)
        {
            if (rb.velocity.y < 0.1f)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
            isJumping = false;
        }
    }

    void ActionJump(bool button)
    {
        if (button)
        {
            isJumping = true;
        }
    }

    #endregion
}
