using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region VARIABLES

    //Variables from PlayerCore
    private PlayerCore playerCore                   = null;
    private ThirdPersonCamera tpCamera              = null;
    private Health health                           = null;

    private float acceleration                      = 0.0f;
    private float airAcceleration                   = 0.0f;
    private float friction                          = 0.0f;
    private float airFriction                       = 0.0f;
    private float gravity                           = 0.0f;
    private float smoothStepDown                    = 0.0f;
    private float jumpForce                         = 0.0f;
    private float jumpGraceTime                     = 0.0f;
    private float dashSpeed                         = 0.0f;
    private float dashJumpForce                     = 0.0f;
    private float dashDuration                      = 0.0f;
    private float dashCooldown                      = 0.0f;
    private LayerMask physicsLayerMask              = 1;
    
    //Input
    [HideInInspector] public bool enableMovement    = true;
    private bool inputJump                          = false;
    private bool inputDash                          = false;
    private Vector2 inputMove                       = Vector2.zero;
    private Vector3 lookDirection                   = Vector3.forward;

    //Temporary values
    private Vector3 moveDirection                   = Vector3.zero;
    private Vector3 moveVector                      = Vector3.zero;
    private Vector3 slopeNormal                     = Vector3.zero;
    private float jumpGraceTimeTemp                 = 0.0f;
    private float dashDurationTemp                  = 0.0f;
    private float dashCooldownTemp                  = 0.0f;
    
    private CharacterController charController      = null;
    private Transform movingPlatform                = null;
    private Vector3 movingPlatformPrevPosition      = Vector3.zero;
    private Vector3 movingPlatformVelocity          = Vector3.zero;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        playerCore          = GetComponent<PlayerCore>();
        charController      = GetComponent<CharacterController>();
        tpCamera            = playerCore.TpcComponent;
        health              = playerCore.HealthComponent;

        acceleration        = playerCore.Acceleration;
        airAcceleration     = playerCore.AirAcceleration;
        friction            = playerCore.Friction;
        airFriction         = playerCore.AirFriction;
        gravity             = playerCore.Gravity;
        smoothStepDown      = playerCore.SmoothStepDown;
        jumpForce           = playerCore.JumpForce;
        jumpGraceTime       = playerCore.JumpGraceTime;
        dashSpeed           = playerCore.DashSpeed;
        dashJumpForce       = playerCore.DashJumpForce;
        dashDuration        = playerCore.DashDuration;
        dashCooldown        = playerCore.DashCooldown;
        physicsLayerMask    = playerCore.PhysicsLayerMask;
    }

    void Update()
    {
        lookDirection = tpCamera.LookDirection;

        if (enableMovement)
        {
            CalculateMovingPlatform();
            Move(inputMove.x, inputMove.y, inputJump, inputDash);
            CalculateCooldowns();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        RaycastHit rcHit;
        if (Physics.Raycast(
            transform.position + Vector3.up * (charController.height / 2),
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
        Debug.DrawLine(hit.point, hit.point + hit.normal * 0.2f, (Mathf.Abs(Vector3.Angle(Vector3.up, hit.normal)) < charController.slopeLimit ? Color.green : Color.red), 0.5f);
        //Vector pointing down the slope
        Debug.DrawLine(hit.point, hit.point + temp2 * 0.2f, Color.blue, 0.5f);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "TriggerKill")
        {
            if (other.GetComponent<TriggerHurt>().killInstantly)
            {
                health.Slay();
            }
            else
            {
                health.Hurt(other.GetComponent<TriggerHurt>().damage);
            }
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void SetInput(string ability, bool b)
    {
        switch (ability)
        {
            case "Jump": inputJump = b; break;
            case "Dash": inputDash = b; break;
            default: break;
        }
    }

    public void SetInput(string ability, Vector2 vector2)
    {
        switch (ability)
        {
            case "Move": inputMove = vector2; break;
            default: break;
        }
    }

    void Move(float x, float y, bool jump, bool dash)
    {
        float moveSpeed = Mathf.Abs(x) + Mathf.Abs(y);
        if (moveSpeed >= 1.0f)
        {
            moveSpeed = 1.0f;
        }

        bool isGrounded = charController.isGrounded;
        if ((charController.collisionFlags & CollisionFlags.Below) == 0)
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
        if (Vector3.Angle(Vector3.up, slopeNormal) < charController.slopeLimit)
        {
            //Calculate movement on XZ plane
            Vector3 tempVector = moveVector;
            tempVector.y = 0.0f;

            if (dashDurationTemp <= 0.0f)
            {
                tempVector += isGrounded ?
                    moveDirection * moveSpeed * acceleration * Time.deltaTime
                    : moveDirection * moveSpeed * airAcceleration * Time.deltaTime;
            }

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
                    charController.skinWidth + smoothStepDown,
                    physicsLayerMask
                    ))
                {
                    tempVector.y = -charController.slopeLimit;
                }
            }
            
            tempVector.y = isGrounded ?
                tempVector.y + gravity * Time.deltaTime
                : moveVector.y + gravity * Time.deltaTime;

            //Dashing
            if (dash && dashCooldownTemp <= 0.0f && dashDurationTemp <= 0.0f)
            {
                dashDurationTemp = dashDuration;
                dashCooldownTemp = dashCooldown;
                tempVector = moveDirection * dashSpeed;
                tempVector.y = dashJumpForce;
            }

            moveVector = tempVector;

            //Jumping
            if (jump && jumpGraceTimeTemp > 0.0f)
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
        if ((charController.collisionFlags & CollisionFlags.Above) != 0 && moveVector.y > 0.0f)
        {
            moveVector.y = 0.0f;
        }

        ////Debug stuff
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

    bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;
        if (Mathf.Abs(Vector3.Angle(v1, v2)) > precision) { equal = false; }
        return equal;
    }

    #endregion
}
