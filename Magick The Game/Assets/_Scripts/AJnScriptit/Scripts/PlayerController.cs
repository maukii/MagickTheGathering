//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    private bool enableControls = true;

    //Camera
    public bool invertY = false;
    public Vector2 sensitivity = new Vector2(1.0f, 1.0f);

    [SerializeField] private Vector2 minMaxPitch = new Vector2(-85.0f, 85.0f);
    [SerializeField] private Transform cameraPivot = null;

    private Vector3 lookDirection = Vector3.zero;

    //Movement

    [SerializeField] private float acceleration = 200.0f;
    [SerializeField] private float airAcceleration = 20.0f;
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float airFriction = 0.02f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float smoothStepDown = 0.5f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveVector = Vector3.zero;
    private Vector3 slopeNormal = Vector3.zero;

    //Abilities

    [SerializeField] private float jumpForce = 15.0f;
    [SerializeField] private float jumpGraceTime = 0.1f;
    [SerializeField] private float dashSpeed = 10.0f;
    [SerializeField] private float dashJumpForce = 5.0f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private float dashCooldown = 2.0f;
    
    private float jumpGraceTimeTemp = 0.0f;
    private float dashDurationTemp = 0.0f;
    private float dashCooldownTemp = 0.0f;

    //Miscellaneous
    [SerializeField] private LayerMask physicsLayerMask;
    [SerializeField] private Text debugText;

    private CharacterController charController;
    private Transform movingPlatform = null;
    private Vector3 movingPlatformPrevPosition = Vector3.zero;
    private Vector3 movingPlatformVelocity = Vector3.zero;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        charController = GetComponent<CharacterController>();
        if (cameraPivot == null)
        {
            Debug.LogWarning(this + " is missing a camera pivot reference!");
            this.enabled = false;
        }
    }

    void OnEnable()
    {
        EnableControls(true);
    }

    void OnDisable()
    {
        EnableControls(false);
    }

    void Update()
    {
        if (enableControls)
        {
            CalculateMovingPlatform();
            ActionLookAround(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            ActionMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Jump"), Input.GetButtonDown("Fire3"));
            CalculateCooldowns();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        slopeNormal = hit.normal;

        if (hit.gameObject.tag == "MovingPlatform")
        {
            movingPlatform = hit.transform;
        }

        Vector2 temp = Vector2.Perpendicular(new Vector2(hit.normal.x, hit.normal.z));
        Vector3 temp2 = Vector3.Normalize(Vector3.Cross(hit.normal, new Vector3(temp.x, 0.0f, temp.y)));
        Debug.DrawLine(hit.point, hit.point + hit.normal * 0.2f, (charController.isGrounded ? Color.green : Color.red), 0.5f);   //Slope normal vector
        Debug.DrawLine(hit.point, hit.point + temp2 * 0.2f, Color.blue, 0.5f);         //Vector pointing down the slope
    }

    #endregion

    #region CUSTOM_METHODS

    public void EnableControls(bool b)
    {
        enableControls = b;
        Cursor.lockState = enableControls ?
            CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !enableControls;
    }

    void ActionLookAround(float x, float y)
    {
        lookDirection.x += y * sensitivity.x * (invertY ? 1.0f : -1.0f);
        lookDirection.y += x * sensitivity.y;

        if (lookDirection.x < minMaxPitch.x) { lookDirection.x = minMaxPitch.x; }
        if (lookDirection.x > minMaxPitch.y) { lookDirection.x = minMaxPitch.y; }

        cameraPivot.localRotation = Quaternion.Euler(lookDirection);
    }

    void ActionMove(float x, float y, bool jump, bool dash)
    {
        bool isGrounded = charController.isGrounded;
        if ((charController.collisionFlags & CollisionFlags.Below) == 0)
        {
            slopeNormal = Vector3.up;
            movingPlatform = null;
        }

        //Get the desired movement unit vector based on where the player is looking at
        Vector3 lookVector = new Vector3(Mathf.Sin(lookDirection.y * Mathf.Deg2Rad), 0.0f, Mathf.Cos(lookDirection.y * Mathf.Deg2Rad));
        Vector3 sideLookVector = Vector3.Cross(lookVector, Vector3.down);
        moveDirection = Vector3.Normalize(lookVector * y + sideLookVector * x);

        //Allow normal movement if not on a slope
        if (Vector3.Angle(Vector3.up, slopeNormal) < charController.slopeLimit)
        {
            //Calculate movement on XZ plane
            Vector3 tempVector = moveVector;
            tempVector.y = 0.0f;

            if (dashDurationTemp <= 0.0f)
            {
                tempVector += isGrounded ?
                    moveDirection * acceleration * Time.deltaTime
                    : moveDirection * airAcceleration * Time.deltaTime;
            }

            //Clamp the speed (not needed, friction already does this for us)
            //if ((Mathf.Abs(tempVector.x) + Mathf.Abs(tempVector.z) / 2) > maxSpeed)
            //{
            //    tempVector = tempVector.normalized * maxSpeed;
            //}

            //Calculate friction
            if (dashDurationTemp <= 0.0f)
            {
                tempVector -= isGrounded ?
                    tempVector * friction
                    : tempVector * airFriction;
            }

            //Calculate movement in Y direction
            if (isGrounded)
            {
                Debug.DrawLine(
                    transform.position + Vector3.up * 0.01f + Vector3.Normalize(tempVector) * charController.radius,
                    transform.position + Vector3.down * (0.01f + smoothStepDown) + Vector3.Normalize(tempVector) * charController.radius,
                    Color.cyan
                );

                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position + Vector3.up * 0.01f + Vector3.Normalize(tempVector) * charController.radius,
                    Vector3.down,
                    out hit,
                    0.01f + smoothStepDown,
                    physicsLayerMask
                    ))
                {
                    tempVector.y = -charController.slopeLimit;
                }
            }
            
            tempVector.y = isGrounded ?
                tempVector.y + Physics.gravity.y * Time.deltaTime
                : moveVector.y + Physics.gravity.y * gravityMultiplier * Time.deltaTime;

            //Dashing
            if (dash && dashCooldownTemp <= 0.0f && dashDurationTemp <= 0.0f)
            {
                dashDurationTemp = dashDuration;
                dashCooldownTemp = dashCooldown;
                tempVector = moveDirection * dashSpeed;
                tempVector.y = dashJumpForce;
            }

            moveVector = tempVector;
        }
        //Do something else when on a steep slope
        else
        {
            //Get a vector pointing downwards a slope
            Vector2 slopeTemp = Vector2.Perpendicular(new Vector2(slopeNormal.x, slopeNormal.z));
            Vector3 slopeTemp2 = Vector3.Normalize(Vector3.Cross(slopeNormal, new Vector3(slopeTemp.x, 0.0f, slopeTemp.y)));

            if (isGrounded)
            {
                moveVector += slopeTemp2 * -Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }

        //Jumping
        if (jump && jumpGraceTimeTemp > 0.0f)
        {
            moveVector.y = jumpForce;
        }

        //Stop vertical movement if hitting a ceiling
        if ((charController.collisionFlags & CollisionFlags.Above) != 0 && moveVector.y > 0.0f)
        {
            moveVector.y = 0.0f;
        }

        //Debug stuff
        Debug.DrawLine(transform.position, transform.position + lookVector, Color.blue);    //Forward vector
        Debug.DrawLine(transform.position, transform.position + sideLookVector, Color.red); //Right vector
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green);   //Up vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveDirection, Color.cyan); //Desired movement unit vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveVector.normalized, Color.magenta); //Desired movement unit vector

        charController.Move(moveVector * Time.deltaTime + movingPlatformVelocity);
    }

    void CalculateMovingPlatform()
    {
        if (movingPlatform != null)
        {
            if (movingPlatformPrevPosition != Vector3.zero)
            {
                movingPlatformVelocity = movingPlatform.position - movingPlatformPrevPosition;
                movingPlatformVelocity.y += Physics.gravity.y * Time.deltaTime;
            }
            movingPlatformPrevPosition = movingPlatform.position;
        }
        else
        {
            movingPlatformPrevPosition = Vector3.zero;
            movingPlatformVelocity = Vector3.zero;
        }
    }

    void CalculateCooldowns()
    {
        if (jumpGraceTimeTemp > 0.0f)
        {
            jumpGraceTimeTemp -= Time.deltaTime;
        }

        if (charController.isGrounded)
        {
            jumpGraceTimeTemp = jumpGraceTime;
        }

        if (dashDurationTemp > 0.0f)
        {
            dashDurationTemp -= Time.deltaTime;
        }
        else
        {
            if (dashCooldownTemp > 0.0f)
            {
                dashCooldownTemp -= Time.deltaTime;
            }
        }
    }

    #endregion
}
