using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float groundDrag;

    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float startYScale;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    [Header("Misc")]
    public bool grounded;
    public bool freeze;
    public bool activeGrapple;
    public bool swinging;
    public bool sliding;
    private bool enableMovementOnNextTouch;

    private RaycastHit slopeHit;

    private Vector3 moveDirection;

    private Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        //List of possible player states
        freeze,
        grappling,
        walking,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Checks if player is in the ground
        grounded = GroundCheck();

        SpeedControl();
        StateHandler();

        //Adds drag if on the ground
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, ground);
    }

    private void StateHandler()
    {
        // Handles movement speed whilst frozen
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        // Handles movement speed whilst grappling
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = grappleSpeed;
        }

        // Handles movemment speed while walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Handles movemment speed while in air/jumping
        else
        {
            state = MovementState.air;
        }

        // Checks if desiredMoveSpeed has changed >8f, if not changes speed instantly
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

        // Gradually changes moveSpeed to desiredMoveSpeed, e.g. sliding down slope to walking speed decreases gradually instead of instant change
        // Velocity increases quicker at greater slope angle
        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void SpeedControl()
    {
        // Limits speed on the ground or in the air
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        // Limits velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

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

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        // Returns Vector3 direction relative to slope angle
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}