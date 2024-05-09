using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraObject;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask grappleSurface;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private PlayerSwinging playerSwinging;

    private PlayerMovement playerMovement;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;

    private SpringJoint joint;
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializeField] private float grappleCooldown;

    private float grappleCooldownTimer;

    [Header("Prediction")]
    [SerializeField] private RaycastHit predictionHit;
    [SerializeField] private float predictionSphereCastRadius;
    [SerializeField] private Transform predictionPoint;

    [Header("Input")]
    [SerializeField] private InputActionReference rightGripInput;

    private float gripInput;
    private bool grappling;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        gripInput = rightGripInput.action.ReadValue<float>();

        if (gripInput >= 0.5f)
            StartGrapple();

        CheckForGrapplePoints();

/*        if (!playerMovement.grounded)
            predictionPoint.gameObject.SetActive(false);*/

        // Cooldown timer for grappling
        if (grappleCooldownTimer > 0)
            grappleCooldownTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        // Updates LineRenderer starting position while grappling, stops player moving through the line
        if (grappling)
            lineRenderer.SetPosition(0, gunTip.position);
    }

    private void CheckForGrapplePoints()
    {
        if (joint != null)
            return;

        Physics.SphereCast(cameraObject.position, predictionSphereCastRadius, cameraObject.forward,
                            out RaycastHit sphereCastHit, maxGrappleDistance, grappleSurface);

        Physics.Raycast(cameraObject.position, cameraObject.forward,
                            out RaycastHit raycastHit, maxGrappleDistance, grappleSurface);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            realHitPoint = raycastHit.point;
        }
        // Option 2 - Indirect (predicted) Hit
        else if (sphereCastHit.point != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            realHitPoint = sphereCastHit.point;
        }
        // Option 3 - Miss
        else
        {
            predictionPoint.gameObject.SetActive(false);
            realHitPoint = Vector3.zero;
        }

        // realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        // realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    private void StartGrapple()
    {
        if (grappleCooldownTimer > 0) return;
        // Return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        grappling = true;
        playerMovement.freeze = true;

        grapplePoint = predictionHit.point;
        Invoke(nameof(ExecuteGrapple), grappleDelayTime);

        // Enables LineRenderer for grappling (makes it visible)
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        playerMovement.freeze = false;

        Vector3 lowestPoint = new(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        // Freezes player, sets grapple state to false, activates cooldown timer, deactivates LineRenderer
        playerMovement.freeze = false;

        grappling = false;

        grappleCooldownTimer = grappleCooldown;

        lineRenderer.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}