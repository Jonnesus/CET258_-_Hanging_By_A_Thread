using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private Transform orientation;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    //[SerializeField] private float sprintSpeed;
    //[SerializeField] private float slideSpeed;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float swingSpeed;
    //[SerializeField] private float maxSlopeAngle;
    [SerializeField] private float speedIncreaseMultiplier;
    //[SerializeField] private float slopeIncreaseMultiplier;
    [SerializeField] private float groundDrag;

    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

/*    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;*/

    //private bool readyToJump;

/*    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;*/

    private float startYScale;

/*    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;*/

    [Header("Ground Check")]
    //[SerializeField] private float playerHeight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    [Header("Misc")]
    public bool grounded;
    public bool freeze;
    public bool activeGrapple;
    public bool swinging;
    public bool sliding;
    //private bool exitingSlope;
    private bool enableMovementOnNextTouch;

    //private float horizontalInput;
    //private float verticalInput;

    private RaycastHit slopeHit;

    private Vector3 moveDirection;

    private Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        //List of possible player states
        freeze,
        grappling,
        swinging,
        walking,
        //sprinting,
        //crouching,
        //sliding,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Checks if player is in the ground
        grounded = GroundCheck();

        //OnSlope();
        //MyInput();
        SpeedControl();
        StateHandler();

        //Adds drag if on the ground
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

/*    private void FixedUpdate()
    {
        MovePlayer();
    }*/

    private bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, ground);
    }

/*    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Allows the player to jump when key is pressed, on ground and cooldown timer is off
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Allows the player to crouch when key is pressed
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Stops crouching when key is released
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }*/

    private void StateHandler()
    {
        //State checks ordered this way to prevent actions such as sprinting whilst crouching

        //Handles movement speed whilst frozen
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        //Handles movement speed whilst grappling
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = grappleSpeed;
        }

        //Handles movenet speed whilst swinging
        else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }

/*        //Handles movemment speed while sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        //Handles movemment speed while crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        //Handles movemment speed while sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }*/

        //Handles movemment speed while walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        //Handles movemment speed while in air/jumping
        else
        {
            state = MovementState.air;
        }

        //Checks if desiredMoveSpeed has changed >8f, if not changes speed instantly
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 8f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        //Gradually changes moveSpeed to desiredMoveSpeed, e.g. sliding down slope to walking speed decreases gradually instead of instant change
        //Velocity increases quicker at greater slope angle
        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

/*            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else*/
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

/*    private void MovePlayer()
    {
        if (activeGrapple) return;
        if (swinging) return;

        //Calculates movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Movement whilst on a slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(moveDirection), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Movement whilst on the ground (AddForce stops floating while crouching)
        else if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);

        //Movement whilst in the air/jumping
        else if (!grounded)
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);

        //Disables gravity whilst on a slope to stop perpetually sliding down
        rb.useGravity = !OnSlope();
    }*/

    private void SpeedControl()
    {
        //Limits speed on a slope
/*        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }*/

        //Limits speed on the ground or in the air
        //else
        //{
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            //Limits velocity
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        //}
    }

/*    private void Jump()
    {
        //Allows player to jump on slopes
        exitingSlope = true;

        //Resets y velocity when jumping
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }*/

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 2f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<PlayerGrappling>().StopGrapple();
        }
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

/*    public bool OnSlope()
    {
        //Checks if player is on a slope. If yes + below maximum slope angle, returns slope angle
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }*/

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        //returns Vector3 direction relative to slope angle
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}