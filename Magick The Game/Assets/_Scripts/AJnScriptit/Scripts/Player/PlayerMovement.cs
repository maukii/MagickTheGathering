﻿using UnityEngine;

[RequireComponent(typeof(PlayerCore))]
public class PlayerMovement : MonoBehaviour
{
    #region VARIABLES
    
    [SerializeField] private float acceleration     = 100.0f;
    [SerializeField] private float airAcceleration  = 20.0f;
    [SerializeField] private float friction         = 0.1f;
    [SerializeField] private float airFriction      = 0.02f;
    [SerializeField] private float gravity          = -30.0f;
    [SerializeField] private float smoothStepDown   = 0.5f;
    [SerializeField] private float jumpForce        = 15.0f;
    [SerializeField] private float jumpGraceTime    = 0.1f;
    [SerializeField] private float dashSpeed        = 20.0f;
    [SerializeField] private float dashJumpForce    = 8.0f;
    [SerializeField] private float dashDuration     = 0.2f;
    [SerializeField] private float dashCooldown     = 1.0f;

    private ThirdPersonCamera cTPCamera             = null;
    private CharacterController cCharacter          = null;
    private LayerMask physicsLayerMask              = 1;

    //Temporary values
    private Vector3 moveDirection                   = Vector3.zero;
    private Vector3 moveVector                      = Vector3.zero;
    private Vector3 slopeNormal                     = Vector3.zero;
    private float jgtTimer                          = 0.0f;
    private float dDurationTimer                    = 0.0f;
    private float dCooldownTimer                    = 0.0f;
    private Transform movingPlatform                = null;
    private Vector3 movingPlatformPrevPosition      = Vector3.zero;
    private Vector3 movingPlatformVelocity          = Vector3.zero;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        cCharacter      = GetComponent<PlayerCore>().cCharacter;
        cTPCamera       = GetComponent<PlayerCore>().cTPCamera;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        RaycastHit rcHit;
        if (Physics.Raycast(
            transform.position + Vector3.up * (cCharacter.height / 2),
            Vector3.down,
            out rcHit,
            Mathf.Infinity,
            physicsLayerMask
            ))
        {
            if (AlmostEqual(hit.normal, rcHit.normal, 0.01f))
            {
                slopeNormal = hit.normal;
            }
            else
            {
                //Most likely standing on stairs
                slopeNormal = Vector3.up;
            }
        }
        else
        {
            //We hit a collider but received nothing from raycast,
            //assume that we hit a wall / ceiling / other
            slopeNormal = Vector3.up;
        }

        if (hit.gameObject.tag == "MovingPlatform")
        {
            movingPlatform = hit.transform;
        }

        Vector2 temp = Vector2.Perpendicular(new Vector2(hit.normal.x, hit.normal.z));
        Vector3 temp2 = Vector3.Normalize(Vector3.Cross(hit.normal, new Vector3(temp.x, 0.0f, temp.y)));
        //Slope normal vector
        Debug.DrawLine(hit.point, hit.point + hit.normal * 0.2f, (Mathf.Abs(Vector3.Angle(Vector3.up, hit.normal)) < cCharacter.slopeLimit ? Color.green : Color.red), 0.5f);
        //Vector pointing down the slope
        Debug.DrawLine(hit.point, hit.point + temp2 * 0.2f, Color.blue, 0.5f);
    }

    #endregion

    #region CUSTOM_METHODS

    public void Move(float x, float y, bool jump, bool dash)
    {
        CalculateMovingPlatform();
        CalculateCooldowns();

        Vector3 lookDirection = cTPCamera.lookDirection;

        float moveSpeed = Mathf.Abs(x) + Mathf.Abs(y);
        if (moveSpeed >= 1.0f)
        {
            moveSpeed = 1.0f;
        }

        bool isGrounded = cCharacter.isGrounded;
        if ((cCharacter.collisionFlags & CollisionFlags.Below) == 0)
        {
            //slopeNormal = Vector3.up;
            movingPlatform = null;
        }

        //Get the desired movement unit vector based on where the player is looking at
        Vector3 lookVector = new Vector3(Mathf.Sin(lookDirection.y * Mathf.Deg2Rad), 0.0f, Mathf.Cos(lookDirection.y * Mathf.Deg2Rad));
        Vector3 sideLookVector = Vector3.Cross(lookVector, Vector3.down);
        moveDirection = Vector3.Normalize(lookVector * y + sideLookVector * x);

        //Get a vector pointing downwards a slope
        Vector2 slopeTemp = Vector2.Perpendicular(new Vector2(slopeNormal.x, slopeNormal.z));
        Vector3 slopeTemp2 = Vector3.Normalize(Vector3.Cross(slopeNormal, new Vector3(slopeTemp.x, 0.0f, slopeTemp.y)));
        
        //Allow normal movement if not on a slope
        if (Vector3.Angle(Vector3.up, slopeNormal) < cCharacter.slopeLimit)
        {
            //Calculate movement on XZ plane
            Vector3 tempVector = moveVector;
            tempVector.y = 0.0f;

            if (dDurationTimer <= 0.0f)
            {
                tempVector += isGrounded ?
                    moveDirection * moveSpeed * acceleration * Time.deltaTime
                    : moveDirection * moveSpeed * airAcceleration * Time.deltaTime;
            }

            //Calculate friction
            if (dDurationTimer <= 0.0f)
            {
                tempVector -= isGrounded ?
                    tempVector * friction
                    : tempVector * airFriction;
            }

            //Calculate movement in Y direction
            if (isGrounded)
            {
                Debug.DrawLine(
                    transform.position + Vector3.up * 0.01f + Vector3.Normalize(tempVector) * cCharacter.radius,
                    transform.position + Vector3.down * (0.01f + smoothStepDown) + Vector3.Normalize(tempVector) * cCharacter.radius,
                    Color.cyan
                );

                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position + Vector3.up * 0.01f + Vector3.Normalize(tempVector) * cCharacter.radius,
                    Vector3.down,
                    out hit,
                    cCharacter.skinWidth + smoothStepDown,
                    physicsLayerMask
                    ))
                {
                    tempVector.y = -cCharacter.slopeLimit;
                }
            }
            
            tempVector.y = isGrounded ?
                tempVector.y + gravity * Time.deltaTime
                : moveVector.y + gravity * Time.deltaTime;

            //Dashing
            if (dash && dCooldownTimer <= 0.0f && dDurationTimer <= 0.0f)
            {
                dDurationTimer = dashDuration;
                dCooldownTimer = dashCooldown;
                tempVector = moveDirection * dashSpeed;
                tempVector.y = dashJumpForce;
            }

            moveVector = tempVector;

            //Jumping
            if (jump && jgtTimer > 0.0f)
            {
                moveVector.y = jumpForce;
            }
        }
        //Do something else when on a steep slope
        else
        {
            Vector3 tempVector = moveDirection * airAcceleration * Time.deltaTime;
            tempVector = Vector3.ProjectOnPlane(tempVector, slopeNormal);
            moveVector = Vector3.ProjectOnPlane(moveVector, slopeNormal);
            moveVector += tempVector + slopeTemp2 * -gravity * Time.deltaTime;

            RaycastHit hit;
            if (!Physics.Raycast(
                transform.position + slopeNormal + moveVector * Time.deltaTime,
                -slopeNormal,
                out hit,
                1.0f + 0.5f,
                physicsLayerMask
                ))
            {
                slopeNormal = Vector3.up;
            }
        }

        //Stop vertical movement if hitting a ceiling
        if ((cCharacter.collisionFlags & CollisionFlags.Above) != 0 && moveVector.y > 0.0f)
        {
            moveVector.y = 0.0f;
        }

        ////Debug stuff
        Debug.DrawLine(transform.position, transform.position + lookVector, Color.blue);    //Forward vector
        Debug.DrawLine(transform.position, transform.position + sideLookVector, Color.red); //Right vector
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green);   //Up vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveDirection, Color.cyan); //Desired movement unit vector
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveVector.normalized, Color.magenta); //Desired movement unit vector

        cCharacter.Move(moveVector * Time.deltaTime + movingPlatformVelocity);
    }

    void CalculateMovingPlatform()
    {
        if (movingPlatform != null)
        {
            if (movingPlatformPrevPosition != Vector3.zero)
            {
                movingPlatformVelocity = movingPlatform.position - movingPlatformPrevPosition;
                movingPlatformVelocity.y += -1.0f * Time.deltaTime;
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
        if (jgtTimer > 0.0f)
        {
            jgtTimer -= Time.deltaTime;
        }

        if (cCharacter.isGrounded)
        {
            jgtTimer = jumpGraceTime;
        }

        if (dDurationTimer > 0.0f)
        {
            dDurationTimer -= Time.deltaTime;
        }
        else
        {
            if (dCooldownTimer > 0.0f)
            {
                dCooldownTimer -= Time.deltaTime;
            }
        }
    }

    bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;
        if (Mathf.Abs(Vector3.Angle(v1, v2)) > precision) { equal = false; }
        return equal;
    }

    #endregion
}
