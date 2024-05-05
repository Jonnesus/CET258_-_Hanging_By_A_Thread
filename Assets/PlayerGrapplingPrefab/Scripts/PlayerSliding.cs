using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    [SerializeField] private float maximumSlideTime = 0.5f;
    [SerializeField] private float slideForce = 200f;
    private float slideTimer;

    [SerializeField] private float slideYScale = 0.5f;
    private float startYScale;

    [Header("Input")]
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        //Sets starting player height
        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        //Controls movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Starts sliding when moving + key is pressed
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();

        //Stops sliding when key is released
        if (Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    public void StartSlide()
    {
        //Sets sliding state to true
        pm.sliding = true;

        //Starts sliding + sets player height whilst sliding
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        //Sets the sliding timer
        slideTimer = maximumSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Sliding when on ground
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            slideTimer -= Time.deltaTime;

            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
        }

        //Sliding when on a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        //Stops sliding once timer has ran out
        if (slideTimer <= 0)
            StopSlide();
    }

    public void StopSlide()
    {
        //Stops sliding and returns player to a walking state
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}