﻿//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    public bool invertY = false;
    public Vector2 sensitivity = new Vector2(1.0f, 1.0f);

    [SerializeField] private float acceleration = 200.0f;
    [SerializeField] private float airAcceleration = 20.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float airFriction = 0.02f;
    [SerializeField] private float jumpForce = 15.0f;
    [SerializeField] private float jumpGraceTime = 0.1f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float smoothStepDown = 0.5f;
    [SerializeField] private Vector2 minMaxPitch = new Vector2(-85.0f, 85.0f);
    [SerializeField] private Transform cameraPivot = null;
    [SerializeField] private LayerMask physicsLayerMask;
    [SerializeField] private Text debugText;

    private bool isJumping = false;
    private float groundedCoyoteTime = 0.0f;
    private const float constGroundedCoyoteTime = 0.1f;

    private float jumpGraceTimeTemp;

    private CharacterController charController;

    private Vector3 lookDirection = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveVector = Vector3.zero;
    private Vector3 slopeNormal = Vector3.zero;

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

    void Update()
    {
        ActionLookAround(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        ActionMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Jump"));
        CalculateJumpGraceTime();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        slopeNormal = hit.normal;
        Vector2 temp = Vector2.Perpendicular(new Vector2(hit.normal.x, hit.normal.z));
        Vector3 temp2 = Vector3.Normalize(Vector3.Cross(hit.normal, new Vector3(temp.x, 0.0f, temp.y)));

        Debug.DrawLine(hit.point, hit.point + hit.normal * 0.2f, Color.green, 0.5f);   //Slope normal vector
        Debug.DrawLine(hit.point, hit.point + temp2 * 0.2f, Color.blue, 0.5f);         //Vector pointing down the slope
    }

    #endregion

    #region CUSTOM_METHODS

    void ActionLookAround(float x, float y)
    {
        lookDirection.x += y * sensitivity.x * (invertY ? 1.0f : -1.0f);
        lookDirection.y += x * sensitivity.y;

        if (lookDirection.x < minMaxPitch.x) { lookDirection.x = minMaxPitch.x; }
        if (lookDirection.x > minMaxPitch.y) { lookDirection.x = minMaxPitch.y; }

        cameraPivot.localRotation = Quaternion.Euler(lookDirection);
    }

    void ActionMove(float x, float y, bool jump)
    {
        #region OLD_METHOD
        /*
        //Walking

        Vector3 lookVector = new Vector3(Mathf.Sin(lookDirection.y * Mathf.Deg2Rad), 0.0f, Mathf.Cos(lookDirection.y * Mathf.Deg2Rad));
        Vector3 sideLookVector = Vector3.Cross(lookVector, Vector3.down);
        //Vector3 charVector = new Vector3(charController.velocity.x, 0.0f, charController.velocity.z);

        //Vector3 combinedVectors = isGrounded ?
        //    combinedVectors = (lookVector * (acceleration * y)) + (sideLookVector * (acceleration * x))
        //    : combinedVectors = (lookVector * (airAcceleration * y)) + (sideLookVector * (airAcceleration * x));

        //moveDirection = (
        //    moveDirection
        //    + lookVector * acceleration * y * Time.deltaTime
        //    + sideLookVector * acceleration * x * Time.deltaTime
        //    );

        //if ((Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z)) > maxSpeed)
        //{
        //    Vector3 clampedVector = moveDirection;
        //    clampedVector.y = 0.0f;
        //    clampedVector = clampedVector.normalized * maxSpeed;

        //    moveDirection.x = clampedVector.x;
        //    moveDirection.z = clampedVector.z;
        //}

        //Jumping

        if (jump && charController.isGrounded)
        {
            moveDirection.y = jumpForce;
        }

        //Add gravity
        moveDirection += Physics.gravity * Time.deltaTime;

        debugText.text = moveDirection.ToString();
        */
        #endregion

        //Get the desired movement unit vector based on where the player is looking at
        Vector3 lookVector = new Vector3(Mathf.Sin(lookDirection.y * Mathf.Deg2Rad), 0.0f, Mathf.Cos(lookDirection.y * Mathf.Deg2Rad));
        Vector3 sideLookVector = Vector3.Cross(lookVector, Vector3.down);
        moveDirection = Vector3.Normalize(lookVector * y + sideLookVector * x);

        //Get a vector pointing downwards a slope
        Vector2 slopeTemp = Vector2.Perpendicular(new Vector2(slopeNormal.x, slopeNormal.z));
        Vector3 slopeTemp2 = Vector3.Normalize(Vector3.Cross(slopeNormal, new Vector3(slopeTemp.x, 0.0f, slopeTemp.y)));

        //Allow normal character movement if not on a slope
        if (charController.isGrounded)
        {
            if (Vector3.Angle(Vector3.up, slopeNormal) < charController.slopeLimit)
            {
                //Calculate movement on XZ plane
                Vector3 tempVector = moveVector;
                tempVector.y = Physics.gravity.y * Time.deltaTime;
                tempVector += moveDirection * acceleration * Time.deltaTime;

                //Calculate friction
                tempVector -= tempVector * friction;

                //If character is going faster than maxSpeed, clamp the speed
                if ((Mathf.Abs(tempVector.x) + Mathf.Abs(tempVector.z)) / 2 > maxSpeed)
                {
                    tempVector = tempVector.normalized * maxSpeed;
                }

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


                //Project on slope plane
                //tempVector = Vector3.ProjectOnPlane(tempVector, slopeNormal);

                moveVector = tempVector;
            }
            else
            {
                moveVector += slopeTemp2 * -Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }
        else
        {
            //Calculate movement on XZ plane
            Vector3 tempVector = moveVector;
            tempVector.y = 0.0f;

            tempVector += moveDirection * airAcceleration * Time.deltaTime;

            //Calculate friction
            tempVector -= tempVector * airFriction;

            //If character is going faster than maxSpeed, clamp the speed
            if ((Mathf.Abs(tempVector.x) + Mathf.Abs(tempVector.z)) / 2 > maxSpeed)
            {
                tempVector = tempVector.normalized * maxSpeed;
            }

            moveVector.x = tempVector.x;
            moveVector.z = tempVector.z;

            moveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }

        //Jumping
        if (jump && isJumping == false)
        {
            moveVector.y = jumpForce;
            isJumping = true;
        }

        Debug.DrawLine(transform.position, transform.position + lookVector, Color.blue);    //Forward vector
        Debug.DrawLine(transform.position, transform.position + sideLookVector, Color.red); //Right vector
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green);   //Up vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveDirection, Color.cyan); //Desired movement unit vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveVector.normalized, Color.magenta); //Desired movement unit vector
        
        charController.Move(moveVector * Time.deltaTime);
        debugText.text = groundedCoyoteTime.ToString();
    }

    void CalculateJumpGraceTime()
    {
        if (jumpGraceTimeTemp > 0.0f)
        {
            jumpGraceTimeTemp -= Time.deltaTime;
        }
        else
        {
            isJumping = true;
        }

        if (charController.isGrounded)
        {
            jumpGraceTimeTemp = jumpGraceTime;
            isJumping = false;
        }
    }

    #endregion
}
